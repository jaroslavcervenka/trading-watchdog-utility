using System.Collections.Generic;
using CommandLine;

namespace Watchdog
{
    public class Options
    {
        [Option('s', "server", Required = true, HelpText = "One or multiple server to connect.")]
        public IEnumerable<string> Servers { get; set; }
        
        [Option('l', "login", Required = true, HelpText = "Server login.")]
        public ulong Login { get; set; }

        [Option('p', "password", Required = true, HelpText = "Server password.")]
        public string Password { get; set; } = "";
        
        [Option('r', "ratio", Required = true, HelpText = "Trade volume to balance ratio.")]
        public decimal Ratio { get; set; }
        
        [Option('d', "delta", Required = true, HelpText = "Open time delta.")]
        public int OpenTimeDelta { get; set; }
        
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        public Options()
        {
            Servers = new List<string>();
        }
    }
    
    
}