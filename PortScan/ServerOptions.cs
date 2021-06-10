using System;
using System.Collections.Generic;
using Fclp;

namespace PortScan
{
    public class ServerOptions
    {
        public string Address { get; set; }
        public bool TcpMode { get; set; }
        public bool UdpMode { get; set; }
        public List<int> Ports { get; set; }

        public static bool TryGetArguments(string[] args, out ServerOptions parsedOptions)
        {
            var argumentsParser = new FluentCommandLineParser<ServerOptions>();

            argumentsParser.Setup(a => a.Address)
                .As(CaseType.CaseInsensitive, "a", "address");

            argumentsParser.Setup(a => a.TcpMode)
                .As(CaseType.CaseInsensitive, "t", "tcpMode");

            argumentsParser.Setup(a => a.UdpMode)
                .As(CaseType.CaseInsensitive, "u", "udpMode");

            argumentsParser.Setup(a => a.Ports)
                .As(CaseType.CaseInsensitive, "p", "ports")
                .Required();

            argumentsParser.SetupHelp("?", "h", "help")
                .Callback(_ =>
                {
                    Console.WriteLine("Example: PortScan.exe -a 127.0.0.1 -tu -p 80 123");
                    Console.WriteLine("-a, --address [ip] - remote ip address [default - 127.0.0.1]");
                    Console.WriteLine("-t, --tcpMode - checking all tcp-ports");
                    Console.WriteLine("-u, --udpMode - checking all udp-ports");
                    Console.WriteLine("-p, --ports N1 N2 - set range for checking, 0 <= N1 <= N2 <= 65535");
                });

            var parsingResult = argumentsParser.Parse(args);
            parsedOptions = argumentsParser.Object;

            if (parsingResult.HasErrors || !parsedOptions.TcpMode && !parsedOptions.UdpMode ||
                !IsRangeCorrect(parsedOptions.Ports))
            {
                argumentsParser.HelpOption.ShowHelp(argumentsParser.Options);
                parsedOptions = null;
                return false;
            }

            return !parsingResult.HasErrors;
        }

        private static bool IsRangeCorrect(IReadOnlyList<int> range) =>
            range.Count == 2 && range[0] >= 0 && range[0] <= range[1] && range[1] <= 65535;
    }
}