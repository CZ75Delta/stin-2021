using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    public class ApiHandler
    {
        public static async Task<string> DownloadFromUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponseAsync().Result;

                if ("gzip".Equals(response.ContentEncoding))
                {
                    Stream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                    var content = await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();
                    return content;
                }
                else
                {
                    var content = await new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEndAsync();
                    return content;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}