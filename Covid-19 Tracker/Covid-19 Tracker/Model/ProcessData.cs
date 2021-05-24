using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    internal class ProcessData
    {
        /// <summary>
        /// Metoda pro vybrání dat z JSON MZCR
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Dictionary
        /// keys - Date, Source, Country, TotalCases (celkový počet nakažených), NewCases, TotalVaccinations, NewVaccinations
        /// </returns>
        public static async Task<Dictionary<string, string>> ProcessMzcr(string json)
        {
            var value = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            var dict = new Dictionary<string, string>
            {
                { "Date", value?["modified"].Date.AddDays(-1).ToString("yyyy-MM-dd") },
                { "Source", "mzcr" },
                { "Country", "Czechia" },
                { "TotalCases", value["data"][0]["potvrzene_pripady_celkem"].ToString() },
                { "NewCases", value["data"][0]["potvrzene_pripady_vcerejsi_den"].ToString() },
                { "TotalVaccinations", value["data"][0]["vykazana_ockovani_celkem"].ToString() }};
            return await Task.FromResult(dict);
        }

        /// <summary>
        /// Metoda pro vybrání dat z CSV WHO CZ
        /// </summary>
        /// <param name="csv"></param>
        /// <returns>Dictionary
        /// keys - Date, Source, Country, TotalCases (celkový počet nakažených), NewCases
        /// </returns>
        public static async Task<Dictionary<string, string>> ProcessWhoInfected(string csv)
        {
            var lines = csv.Split('\n');
            var dateYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            foreach (var line in lines)
            {
                if (!line.StartsWith(dateYesterday)) continue;
                if (!line.Contains("CZ")) continue;
                var data = line.Split(',');
                var dict = new Dictionary<string, string>
                {
                    {"Date", dateYesterday},
                    {"Source", "who"},
                    {"Country", "Czechia"},
                    {"TotalCases", data[5]},
                    {"NewCases", data[4]}
                };
                return await Task.FromResult(dict);
            }

            return null;
        }

        /// <summary>
        /// Metoda vybrání dat z WHO Země
        /// </summary>
        /// <param name="csv"></param>
        /// <returns>list zemí
        /// dictionary
        /// keys - Date, Source, Country, IsoCode,TotalVaccinations
        /// </returns>
        public static async Task<List<Dictionary<string, string>>> ProcessWhoVaccinated(string csv)
        {
            var list = new List<Dictionary<string, string>>();
            var lines = csv.Split("\r\n");
            for (var i = 1; i < lines.Length - 1; i++)
            {
                var regex = new Regex(',' + "(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                var data = regex.Split(lines[i]);

                //var data = lines[i].Split(',');
                var dict = new Dictionary<string, string>
                {
                    {"Date", data[4]},
                    {"Source", "who"},
                    {"Country", data[0].Trim('\"')},
                    {"IsoCode", data[1]},
                    {"TotalVaccinations", data[6].Equals("") ? data[5].Equals("") ? "0" : (int.Parse(data[5])/2).ToString() : data[6]}
                };
                list.Add(dict);
            }
            return await Task.FromResult(list);
        }
    }
}
