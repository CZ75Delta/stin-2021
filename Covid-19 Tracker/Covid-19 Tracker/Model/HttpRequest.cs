using System;
using System.Net;
using Newtonsoft.Json;

namespace Covid_19_Tracker.Model
{
    public class HttpRequest
    {
        public string Data { get; }

        private HttpRequest(string url)
        {
            using var webClient = new WebClient();
            Data = string.Empty;
            try
            {
                Data = webClient.DownloadString(url);
            }
            catch (Exception)
            {
                // ignored
            }

            
        }
    }
}