using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public static class TcpScanner
    {
        private static readonly Dictionary<int, string> SpecialPorts = new()
        {
            [80] = "HTTP",
            [25] = "SMTP",
            [587] = "SMTP",
            [465] = "SMTP",
            [110] = "POP3",
            [135] = "EPMAP",
            [995] = "POP3",
            [143] = "IMAP",
            [993] = "IMAP"
        };
        
        public static async Task Scan(IPAddress address, int[] ports) => await CheckAllPortsAsync(address, ports);

        private static async Task CheckAllPortsAsync(IPAddress address, int[] ports)
        {
            var pingAddr = await PingAddrAsync(address);
            if (pingAddr != IPStatus.Success)
                return;

            await Task.WhenAll(ports.Select(p => CheckPortAsync(address, p)));
        }

        private static async Task<IPStatus> PingAddrAsync(IPAddress ipAddr, int timeout = 3000)
        {
            using var ping = new Ping();
            var statusTask = await ping.SendPingAsync(ipAddr, timeout);
            return statusTask.Status;
        }

        private static async Task CheckPortAsync(IPAddress ipAddr, int port, int timeout = 3000)
        {
            using var tcpClient = new TcpClient();

            var connectTask = await tcpClient.ConnectWithTimeoutAsync(ipAddr, port, timeout);
            
            if (connectTask.Status == TaskStatus.RanToCompletion)
                Console.WriteLine($"TCP {port} {(SpecialPorts.ContainsKey(port) ? SpecialPorts[port] : "")}");
        }
    }
}