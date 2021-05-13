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
            //Date, Source, Country, TotalCases (aktuální počet nakažených), NewCases, TotalVaccinations, NewVaccinations
            Dictionary<string, string> dict = new Dictionary<string, string>();

            dict.Add("Date", value["modified"].Date.ToString("dd/MM/yyyy"));
            dict.Add("Source", "mzcr");
            dict.Add("Country", "cze");
            dict.Add("TotalCases", value["data"][0]["aktivni_pripady"].ToString());
            dict.Add("NewCases", value["data"][0]["potvrzene_pripady_vcerejsi_den"].ToString());
            dict.Add("TotalVaccinations", value["data"][0]["vykazana_ockovani_celkem"].ToString());
            dict.Add("NewVaccinations", value["data"][0]["vykazana_ockovani_vcerejsi_den"].ToString());

            return dict;
        }

    }
}
