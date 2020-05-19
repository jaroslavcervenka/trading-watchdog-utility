using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;

namespace Watchdog.Worker.Model
{
    public class AppOptions
    {
        public IEnumerable<string> Servers { get; private set; }
        
        public float Ratio { get; private set; }
        
        public float OpenTimeDelta { get; private set; }

        public AppOptions(
            IEnumerable<string> servers,
            float ratio,
            float openTimeDelta)
        {
            Guard.Against.Null(servers, nameof(servers));

            if (!servers.Any())
            {
                throw new ArgumentException("At least one server should be set", nameof(servers));
            }

            Guard.Against.Zero(ratio, nameof(ratio));
            Guard.Against.Zero(openTimeDelta, nameof(openTimeDelta));

            Servers = servers;
            Ratio = ratio;
            OpenTimeDelta = openTimeDelta;
        }
    }
}