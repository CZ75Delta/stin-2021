using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    class ProcessData
    {
        /// <summary>
        /// Metoda pro vybrání dat z JSON MZCR
        /// </summary>
        /// <param name="json"></param>
        /// <returns>Dictionary
        /// keys - Date, Source, Country, TotalCases (celkový počet nakažených), NewCases, TotalVaccinations, NewVaccinations
        /// </returns>
        public Dictionary<string, string> JSONToDictMZCR(string json)
        {
            var value = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("Date", value["modified"].Date.AddDays(-1).ToString("yyyy-MM-dd"));
            dict.Add("Source", "mzcr");
            dict.Add("Country", "Czechia");
            dict.Add("TotalCases", value["data"][0]["potvrzene_pripady_celkem"].ToString());
            dict.Add("NewCases", value["data"][0]["potvrzene_pripady_vcerejsi_den"].ToString());
            dict.Add("TotalVaccinations", value["data"][0]["vykazana_ockovani_celkem"].ToString());
            dict.Add("NewVaccinations", value["data"][0]["vykazana_ockovani_vcerejsi_den"].ToString());

            return dict;
        }

        /// <summary>
        /// Metoda pro vybrání dat z CSV WHO CZ
        /// </summary>
        /// <param name="csv"></param>
        /// <returns>Dictionary
        /// keys - Date, Source, Country, TotalCases (celkový počet nakažených), NewCases
        /// </returns>
        public Dictionary<string, string> CSVToDictWHOCR(string csv)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string[] lines = csv.Split('\n');
            string dateYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            foreach (string line in lines)
            {
                if (line.StartsWith(dateYesterday))
                {
                    if (line.Contains("CZ"))
                    {
                        string[] data = line.Split(',');
                        dict.Add("Date", dateYesterday);
                        dict.Add("Source", "who");
                        dict.Add("Country", "Czechia");
                        dict.Add("TotalCases", data[5]);
                        dict.Add("NewCases", data[4]);
                        break;
                    }
                }
            }
            
            return dict;
        }
        /// <summary>
        /// Metoda vybrání dat z WHO Země
        /// </summary>
        /// <param name="csv"></param>
        /// <returns>list zemí
        /// dictionary
        /// keys - Date, Source, Country, TotalVaccinations
        /// </returns>
        public List<Dictionary<string, string>> CSVToListWHOCountries(string csv)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            Dictionary<string, string> dict;
            string[] lines = csv.Split("\r\n");

            for (int i = 1; i < lines.Length - 1; i++)
            {
                string[] data = lines[i].Split(',');
                dict = new Dictionary<string, string>();

                dict.Add("Date", data[3]);
                dict.Add("Source", "who");
                dict.Add("Country", data[0]);

                if (data[6].ToString().Equals(""))
                {
                    dict.Add("TotalVaccinations", "0");
                }
                else { dict.Add("TotalVaccinations", data[6]); }

                list.Add(dict);
            }
            return list;
        }
    }
}
