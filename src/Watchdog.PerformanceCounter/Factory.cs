using System;
using Qoollo.PerformanceCounters.WinCounters;

namespace Watchdog.PerformanceCounter
{
    public class Factory : IDisposable
    {
        private const string RootName = "Watchdog";
        private const string RootDescription = "console app";
        
        private WinCounterFactory _counterFactory;
        
        public void Init()
        {
            _counterFactory = new WinCounterFactory();
            PerfCounters.Init(_counterFactory.CreateRootWrapper());
            _counterFactory.InitAll();
        }

        public void Dispose()
        {
            _counterFactory?.Dispose();
        }
    }
}