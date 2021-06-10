using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PortScan
{
    public static class UdpScanner
    {
        private static readonly Dictionary<int, string> SpecialPorts = new()
        {
            [53] = "DNS",
            [123] = "SNTP"
        };
        public static async Task Scan(IPAddress address, int[] ports) => await CheckAllPortsAsync(address, ports);

        private static async Task CheckAllPortsAsync(IPAddress address, int[] ports) => 
            await Task.WhenAll(ports.Select(p => PingAddrAsync(address, p)));

        private static async Task PingAddrAsync(IPAddress ipAddr, int port, int timeout = 3000)
        {
            var waiter = Task.Delay(timeout);
            var resultTask = await Task.WhenAny(waiter, CheckPortAsync(ipAddr, port));
            if (resultTask == waiter)
                return;

            var status = ((Task<PortStatus>) resultTask).Result;
            if (status != PortStatus.Open)
                return;
            Console.WriteLine($"UDP {port} {(SpecialPorts.ContainsKey(port) ? SpecialPorts[port] : "")}");
        }

        private static async Task<PortStatus> CheckPortAsync(IPAddress ipAddr, int port)
        {
            using var udpClient = new UdpClient();
            try
            {
                udpClient.Connect(ipAddr, port);
                return PortStatus.Open;
            }
            catch (SocketException error)
            {
                if (error.ErrorCode == 10054)
                    return PortStatus.Closed;
            }

            return PortStatus.Filtered;
        }
    }
}