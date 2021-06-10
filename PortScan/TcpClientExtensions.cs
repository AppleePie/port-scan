using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public static class TcpClientExtensions
    {
        public static async Task<Task> ConnectWithTimeoutAsync(this TcpClient tcpClient, IPAddress ipAddr, int port, int timeout = 3000)
        {
            var connectTask = Task.Run(() => { });
            
            try
            {
                connectTask = tcpClient.ConnectAsync(ipAddr, port);
            }
            catch
            {
                // ignored
            }

            await Task.WhenAny(connectTask, Task.Delay(timeout));
            return connectTask;
        }
    }
}