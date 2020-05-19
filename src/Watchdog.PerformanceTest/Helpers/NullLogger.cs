using System;
using Microsoft.Extensions.Logging;

namespace Watchdog.PerformanceTest.Helpers
{
    public class NullLogger<T> : ILogger<T>, IDisposable
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {

        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }
            
        public void Dispose()
        {
        }
    }
}