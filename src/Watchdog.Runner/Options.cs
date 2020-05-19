using System.Collections.Generic;
using CommandLine;

namespace Watchdog.Runner
{
    public class Options
    {
        [Option('s', "server", Required = true, HelpText = "One or multiple server to connect.")]
        public IEnumerable<string> Servers { get; set; }
        
        [Option('r', "ratio", Required = true, HelpText = "Trade volume to balance ratio.")]
        public float Ratio { get; set; }
        
        [Option('d', "delta", Required = true, HelpText = "Open time delta.")]
        public float OpenTimeDelta { get; set; }
        
        [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
        public bool Verbose { get; set; }

        public Options()
        {
            Servers = new List<string>();
        }
    }
    
    
}