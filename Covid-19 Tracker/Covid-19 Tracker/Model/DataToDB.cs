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
        public void DictToDb(Dictionary<string, string> dict)
        {
            using var ctx = new TrackerDbContext();
            Infected infected = new Infected();
            Vaccinated vaccinated = new Vaccinated();

            // mzcr dokument
            if (dict.ContainsKey("TotalCases"))
            {
                infected.Source = dict["Source"];
                infected.TotalCases = int.Parse(dict["TotalCases"]);
                infected.NewCases = int.Parse(dict["NewCases"]);
                infected.Date = Convert.ToDateTime(dict["Date"]);
            }
            // who dokument
            if (dict.ContainsKey("TotalVaccinations"))
            {
                vaccinated.Source = dict["Source"];
                vaccinated.TotalVaccinations = int.Parse(dict["TotalVaccinations"]);
                vaccinated.Date = Convert.ToDateTime(dict["Date"]);
            }


            var country = ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]);
            if (country != null)
            {
                infected.Country = country;
                vaccinated.Country = country;
            }
            else
            {
                Country countryNew = new Country();
                countryNew.Name = dict["Country"];
                countryNew.IsoCode = dict["IsoCode"];
                ctx.Countries.Add(countryNew);
            }

            ctx.Infected.Add(infected);
            ctx.Vaccinated.Add(vaccinated);
            ctx.SaveChanges();
        }


        /// <summary>
        /// Aktualizuje data v databázi o populacích všech zemí
        /// </summary>
        public void UpdatePopulation()
        {
            using var ctx = new TrackerDbContext();
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
            ctx.SaveChangesAsync();
            reply = client.DownloadString("https://restcountries.eu/rest/v2/alpha/" + "?codes=" + goodCodes + "&fields=population;alpha3Code");
            var countriesPopulation = JsonConvert.DeserializeObject<List<CountryPopulation>>(reply);
            foreach (var country in countriesPopulation)
            {
                var updatedCountry = ctx.Countries.FirstOrDefaultAsync(x => x.IsoCode == country.Alpha3Code).Result;
                if (updatedCountry == null) continue;
                updatedCountry.Population = country.Population;
                ctx.Countries.Update(updatedCountry);
            }
            ctx.SaveChangesAsync();
        }
    }
    public class CountryPopulation
    {
        public long Population { get; set; }
        public string Alpha3Code { get; set; }
    }
}