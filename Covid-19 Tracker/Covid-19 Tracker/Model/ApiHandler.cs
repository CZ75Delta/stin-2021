using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace Covid_19_Tracker.Model
{
    public class ApiHandler
    {
        public string DownloadFromUrl(string url)
        {
            using var webClient = new WebClient();
            try
            {
                var data = webClient.DownloadString(url);
                return data;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Upravit toto aby zvládnula načíst nějak normálně toto
        /// https://covid19.who.int/who-data/vaccination-data.csv
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string DownloadCSVFromUrl(string url)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();


                StreamReader sr = new StreamReader(resp.GetResponseStream(),Encoding.Default, false);
                string results = sr.ReadToEnd();
                sr.Close();

                return results;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}