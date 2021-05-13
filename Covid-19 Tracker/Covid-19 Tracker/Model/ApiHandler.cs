using System;
using System.Net;
using Newtonsoft.Json;

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
    }
}