using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Serilog;

namespace Covid_19_Tracker.Model
{
    public class CheckInternetConnection
    {
        public Task<bool> CheckForInternetConnection(int timeoutMs = 10000, string url = null)
        {
            try
            {
                url ??= CultureInfo.InstalledUICulture switch
                {
                    { Name: var n } when n.StartsWith("fa") => // Iran
                        "http://www.aparat.com",
                    { Name: var n } when n.StartsWith("zh") => // China
                        "http://www.baidu.com",
                    _ =>
                        "http://www.gstatic.com/generate_204",
                };

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.Timeout = timeoutMs;
                using var response = (HttpWebResponse)request.GetResponseAsync().Result;
                Log.Information("Connection established.");
                return Task.FromResult(true);
            }
            catch
            {
                Log.Error("No Internet Connection");
                return Task.FromResult(false);
            }
        }
    }
}