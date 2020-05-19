using Ardalis.GuardClauses;
using MT5Wrapper.Interface;
using Watchdog.Worker.Model;

namespace Watchdog.Worker
{
    public class App
    {
        private readonly AppOptions _options;

        public App(AppOptions options)
        {
            Guard.Against.Null(options, nameof(options));
            _options = options;
        }
        
        public void Run()
        {
            
        }
    }
}