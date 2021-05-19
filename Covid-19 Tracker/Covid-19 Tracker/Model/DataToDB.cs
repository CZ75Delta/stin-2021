using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;

namespace Covid_19_Tracker.Model
{
    public class DataToDb
    {
        /// <summary>
        /// Metoda pro zpracování dat do objektů databáze a následné přidání, či aktualizaci
        /// </summary>
        /// <param name="dict"></param>
        public async void DictToDb(Dictionary<string, string> dict)
        {
            var infected = new Infected();
            var vaccinated = new Vaccinated();
            await using var ctx = new TrackerDbContext();

            if (dict.ContainsKey("TotalCases"))
            {
                infected.Source = dict["Source"];
                infected.TotalCases = int.Parse(dict["TotalCases"]);
                infected.NewCases = int.Parse(dict["NewCases"]);
                infected.Date = Convert.ToDateTime(dict["Date"]);
            }
            
            if (dict.ContainsKey("TotalVaccinations"))
            {
                vaccinated.Source = dict["Source"];
                vaccinated.TotalVaccinations = int.Parse(dict["TotalVaccinations"]);
                vaccinated.Date = Convert.ToDateTime(dict["Date"]);
            }

            // přidání státu do DB pokud neexistuje
            var country = ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]);

            // přidání do DB pokud není null
            if (infected.Source != null)
            {
                if (country != null) infected.CountryId = country.Id;
                await ctx.Infected.AddAsync(infected);
            }
            if (vaccinated.Source != null)
            {
                if (country != null) vaccinated.CountryId = country.Id;
                await ctx.Vaccinated.AddAsync(vaccinated);
            }

            await ctx.SaveChangesAsync();
        }

        public async void InitializeCountries(List<Dictionary<string, string>> list)
        {
            await using var ctx = new TrackerDbContext();
            foreach (var record in list)
            {
                var newCountry = new Country
                {
                    Name = record["Country"],
                    IsoCode = record["IsoCode"]
                };
                await ctx.Countries.AddAsync(newCountry);
            }
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Aktualizuje data v databázi o populacích všech zemí
        /// </summary>
        public async void UpdatePopulation()
        {
            await using var ctx = new TrackerDbContext();
            using var client = new WebClient();
            var goodCodes = "";
            string reply;
            foreach (var country in ctx.Countries.ToList())
            {
                if (country.IsoCode.StartsWith("X"))
                {
                    reply = client.DownloadString("https://restcountries.eu/rest/v2/name/" + country.Name + "?fields=population");
                    var countryPopulation = JsonConvert.DeserializeObject<List<CountryPopulation>>(reply);
                    country.Population = countryPopulation[0].Population;
                    ctx.Countries.Update(country);
                }
                else
                {
                    goodCodes += country.IsoCode + ";";
                }
            }
            await ctx.SaveChangesAsync();
            reply = client.DownloadString("https://restcountries.eu/rest/v2/alpha/" + "?codes=" + goodCodes + "&fields=population;alpha3Code");
            var countriesPopulation = JsonConvert.DeserializeObject<List<CountryPopulation>>(reply);
            foreach (var country in countriesPopulation)
            {
                var updatedCountry = ctx.Countries.FirstOrDefaultAsync(x => x.IsoCode == country.Alpha3Code).Result;
                if (updatedCountry == null) continue;
                updatedCountry.Population = country.Population;
                ctx.Countries.Update(updatedCountry);
            }
            await ctx.SaveChangesAsync();
        }
    }
    public class CountryPopulation
    {
        public long Population { get; set; }
        public string Alpha3Code { get; set; }
    }
}