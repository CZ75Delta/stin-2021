using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Covid_19_Tracker.Model
{
    public class DataToDb
    {
        /// <summary>
        /// Metoda pro přidání jednoho záznamu do databáze
        /// </summary>
        /// <param name="dict">Dictionary se záznamem, který chceme přidat do databáze</param>
        public async Task SavetoDb(Dictionary<string, string> dict)
        {
            var infected = new Infected();
            var vaccinated = new Vaccinated();
            await using var ctx = new TrackerDbContext();

            if (ctx.Countries.Any(x => x.Name == dict["Country"]))
            {
                var country = ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]);

                if (dict.ContainsKey("TotalCases") && !ctx.Infected.Any(x => x.Source.Equals(dict["Source"]) && x.Date.Equals(Convert.ToDateTime(dict["Date"])) && x.CountryId.Equals(country.Id)))
                {
                    infected.Source = dict["Source"];
                    infected.TotalCases = int.Parse(dict["TotalCases"]);
                    infected.NewCases = int.Parse(dict["NewCases"]);
                    infected.Date = Convert.ToDateTime(dict["Date"]);
                    if (country != null) infected.CountryId = country.Id;
                    await ctx.Infected.AddAsync(infected);
                }

                if (dict.ContainsKey("TotalVaccinations") && !ctx.Vaccinated.Any(x => x.Source.Equals(dict["Source"]) && x.Date.Equals(Convert.ToDateTime(dict["Date"])) && x.CountryId.Equals(country.Id)))
                {
                    vaccinated.Source = dict["Source"];
                    vaccinated.TotalVaccinations = int.Parse(dict["TotalVaccinations"]);
                    vaccinated.Date = Convert.ToDateTime(dict["Date"]);
                    if (country != null) vaccinated.CountryId = country.Id;
                    await ctx.Vaccinated.AddAsync(vaccinated);
                }
            }

            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Metoda pro přidání více záznamů do databáze
        /// </summary>
        /// <param name="dict">List Dictionary se záznamy, které chceme přidat do databáze</param>
        public async Task SavetoDb(IEnumerable<Dictionary<string, string>> list)
        {
            await using var ctx = new TrackerDbContext();

            foreach (var dict in list)
            {
                var infected = new Infected();
                var vaccinated = new Vaccinated();
                if (ctx.Countries.Any(x => x.Name == dict["Country"]))
                {
                    var country = ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]);

                    if (dict.ContainsKey("TotalCases") && !ctx.Infected.Any(x => x.Source.Equals(dict["Source"]) && x.Date.Equals(Convert.ToDateTime(dict["Date"])) && x.CountryId.Equals(country.Id)))
                    {
                        infected.Source = dict["Source"];
                        infected.TotalCases = int.Parse(dict["TotalCases"]);
                        infected.NewCases = int.Parse(dict["NewCases"]);
                        infected.Date = Convert.ToDateTime(dict["Date"]);
                        if (country != null) infected.CountryId = country.Id;
                        await ctx.Infected.AddAsync(infected);
                    }

                    if (dict.ContainsKey("TotalVaccinations") && !ctx.Vaccinated.Any(x => x.Source.Equals(dict["Source"]) && x.Date.Equals(Convert.ToDateTime(dict["Date"])) && x.CountryId.Equals(country.Id)))
                    {
                        vaccinated.Source = dict["Source"];
                        vaccinated.TotalVaccinations = int.Parse(dict["TotalVaccinations"]);
                        vaccinated.Date = Convert.ToDateTime(dict["Date"]);
                        if (country != null) vaccinated.CountryId = country.Id;
                        await ctx.Vaccinated.AddAsync(vaccinated);
                    }
                }
            }
            await ctx.SaveChangesAsync();
        }

        /// <summary>
        /// Inicializuje databázi zemí
        /// </summary>
        /// <param name="list">WHO List zemí</param>
        public async Task InitializeCountries(IEnumerable<Dictionary<string, string>> list)
        {
            await using var ctx = new TrackerDbContext();
            foreach (var record in list)
            {
                if (ctx.Countries.Any(x => x.Name == record["Country"])) continue;
                var newCountry = new Country
                {
                    Name = record["Country"],
                    IsoCode = record["IsoCode"]
                };
                await ctx.Countries.AddAsync(newCountry);
            }
            await ctx.SaveChangesAsync();
            await UpdatePopulation();
        }

        /// <summary>
        /// Aktualizuje data v databázi o populacích všech zemí
        /// </summary>
        private async Task UpdatePopulation()
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