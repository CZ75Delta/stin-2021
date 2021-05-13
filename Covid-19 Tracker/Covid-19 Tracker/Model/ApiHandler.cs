using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

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
                
                Stream stream = new GZipStream(resp.GetResponseStream(), CompressionMode.Decompress);
                string content = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                return content;
            }
            catch (Exception)
            {
                return string.Empty;
            }
            
        }
    }
}