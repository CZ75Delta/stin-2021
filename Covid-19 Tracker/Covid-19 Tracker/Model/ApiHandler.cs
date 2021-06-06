using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    public static class ApiHandler
    {
        public static async Task<string> DownloadFromUrl(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);
                request.Timeout = 10000;
                request.ReadWriteTimeout = 10000;
                var response = (HttpWebResponse) await request.GetResponseAsync();

                if (!"gzip".Equals(response.ContentEncoding)) return await new StreamReader(response.GetResponseStream(), Encoding.UTF8).ReadToEndAsync();
                Stream stream = new GZipStream(response.GetResponseStream(), CompressionMode.Decompress);
                return await new StreamReader(stream, Encoding.UTF8).ReadToEndAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}