using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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
            Infected infected = new Infected();
            Vaccinated vaccinated = new Vaccinated();
            using var ctx = new TrackerDbContext();
            
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
            if (country == null)
            {
                Country countryNew = new Country();
                countryNew.Name = dict["Country"];
                countryNew.IsoCode = dict["IsoCode"];
                ctx.Countries.Add(countryNew);
            }

            // přidání do DB pokud není null
            if (infected.Source != null)
            {
                infected.Country = country;
                ctx.Infected.Add(infected);
            }
            if (vaccinated.Source != null)
            {
                vaccinated.Country = country;
                ctx.Vaccinated.Add(vaccinated);
            }
            
            ctx.SaveChanges();
        }


        // TODO dodělat aktualizaci populací (u všech) - volání v mainViewModel po aktualizaci všech ostatních dat
        /// <summary>
        /// Aktualizuje data v databázi o populacích všech zemí
        /// </summary>
        public void UpdatePopulation()
        {
            using var ctx = new TrackerDbContext();
            foreach (var record in ctx.Countries.ToList())
            {
                //TODO - najít populaci země a uložit do record
                ctx.Update(record);
            }

            ctx.SaveChanges();
        }
    }
}