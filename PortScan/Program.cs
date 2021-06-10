using System.Linq;
using System.Net;

namespace PortScan
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (!ServerOptions.TryGetArguments(args, out var parsedOptions))
                return;
            
            if (!IPAddress.TryParse(parsedOptions.Address, out var address))
                address = IPAddress.Loopback;
            
            var ports = Enumerable
                .Range(parsedOptions.Ports[0], parsedOptions.Ports[1] - parsedOptions.Ports[0] + 1)
                .ToArray();
            if (parsedOptions.TcpMode)
                TcpScanner.Scan(address, ports).Wait();
            if (parsedOptions.UdpMode)
                UdpScanner.Scan(address, ports).Wait();
        }
    }
}