using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.GuardClauses;

namespace Watchdog.App.Model
{
    public class AppOptions
    {
        public IEnumerable<string> Servers { get; }

        public ulong Login { get; }

        public string Password { get; }

        public decimal Ratio { get; }
        
        public int OpenTimeDelta { get; }

        public AppOptions(
            IEnumerable<string> servers,
            ulong login,
            string password,
            decimal ratio,
            int openTimeDelta)
        {
            Guard.Against.Null(servers, nameof(servers));
            Guard.Against.Zero((long)login, nameof(login));
            Guard.Against.NullOrEmpty(password, nameof(password));

            if (!servers.Any())
            {
                throw new ArgumentException("At least one server should be set", nameof(servers));
            }

            Guard.Against.Zero(ratio, nameof(ratio));
            Guard.Against.Zero(openTimeDelta, nameof(openTimeDelta));

            Servers = servers;
            Login = login;
            Password = password;
            Ratio = ratio;
            OpenTimeDelta = openTimeDelta;
        }
    }
}