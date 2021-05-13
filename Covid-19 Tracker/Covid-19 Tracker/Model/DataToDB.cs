using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Covid_19_Tracker.Model
{
    public class DataToDb
    {
        /// <summary>
        /// Metoda pro zpracování dat do objektů databáze a následné poslání,
        /// update či přidání nového záznamu pokud existuje
        /// </summary>
        /// <param name="dict"></param>
        public void DictDataToDb(Dictionary<string, string> dict)
        {
            // TODO - WIP - přiřadit hodnoty z dict do objektů
            bool toAdd;
            using var ctx = new TrackerDbContext();
            Infected infected = new Infected();
            Vaccinated vaccinated = new Vaccinated();

            var country = ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]);
            if (ctx.Countries.FirstOrDefault(x => x.Name == dict["Country"]) != null)
            {
                toAdd = false;
            }
            else
            {
                toAdd = true;
            }

            infected.Country = country;
            
            
            
            if (toAdd == true)
            {
                infected.Country = country;
                //ctx.Countries.Update();
            }
            else
            {
                //ctx.Countries.Add();
            }
        }

        // TODO dodělat aktualizaci populací (u všech) - volání v mainViewModel po aktualizaci všech ostatních dat
        public void UpdatePopulation()
        {
            
        }
    }
}