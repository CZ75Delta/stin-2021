using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Covid_19_Tracker.Model
{
    public class IdentifyComputer
    {
        public static async Task<string> GetIdentification()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0);
            await socket.ConnectAsync("8.8.8.8", 65530);
            var endPoint = socket.LocalEndPoint as IPEndPoint;
            return new WebClient().DownloadString("http://icanhazip.com").Replace("\n", "") + "/" + endPoint?.Address;
        }
    } 
}
