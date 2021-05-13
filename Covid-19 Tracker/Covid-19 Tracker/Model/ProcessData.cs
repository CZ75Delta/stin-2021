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
        public Dictionary<string, string> JSONToDictMZCR(string json)
        {
            var value = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(json);
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("Date", value["modified"].Date.AddDays(-1).ToString("yyyy-MM-dd"));
            dict.Add("Source", "mzcr");
            dict.Add("Country", "cze");
            dict.Add("TotalCases", value["data"][0]["potvrzene_pripady_celkem"].ToString());
            dict.Add("NewCases", value["data"][0]["potvrzene_pripady_vcerejsi_den"].ToString());
            dict.Add("TotalVaccinations", value["data"][0]["vykazana_ockovani_celkem"].ToString());
            dict.Add("NewVaccinations", value["data"][0]["vykazana_ockovani_vcerejsi_den"].ToString());

            //Date, Source, Country, TotalCases (celkový počet nakažených), NewCases, TotalVaccinations, NewVaccinations
            return dict;
        }

        public Dictionary<string, string> CSVToDictWHOCR(string csv)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            string[] lines = csv.Split('\n');
            string dateYesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            foreach(string line in lines)
            {
                if (line.StartsWith(dateYesterday))
                {
                    if (line.Contains("CZ")){
                        string[] data = line.Split(',');
                        dict.Add("Date", dateYesterday);
                        dict.Add("Source", "who");
                        dict.Add("Country", "cze");
                        dict.Add("TotalCases", data[5]);
                        dict.Add("NewCases", data[4]);
                        break;
                    }
                }
            }
            //Date, Source, Country, TotalCases (celkový počet nakažených), NewCases
            return dict;
        }

        public List<Dictionary<string,string>> CSVToListWHOCountries(string csv)
        {
            throw new NotImplementedException();
        }
    }
}
