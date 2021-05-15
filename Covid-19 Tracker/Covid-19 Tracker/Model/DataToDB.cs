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
                vaccinated.NewVaccinations = int.Parse(dict["NewVaccinations"]);
                infected.Date = Convert.ToDateTime(dict["Date"]);
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
                ctx.Countries.Add(countryNew);
            }

            ctx.Infected.Add(infected);
            ctx.Vaccinated.Add(vaccinated);
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