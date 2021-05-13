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
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();

                if ("gzip".Equals(response.ContentEncoding))
                {
                    Stream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                    var content = new StreamReader(stream, Encoding.UTF8).ReadToEnd();
                    return content;
                }
                else
                {
                    var content =  new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEnd();
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