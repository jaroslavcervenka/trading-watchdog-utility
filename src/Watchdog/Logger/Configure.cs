using Serilog;
using Serilog.Events;

namespace Watchdog.Logger
{
    internal static class Configure
    {
        private const string LogOutputTemplate = "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}] [{ThreadId}] {Message}{NewLine}{Exception}";
        private const string LogFileName = "watchdog.log";
        
        internal static void CreateLogger(bool verbose)
        {
            var logEventLevel = verbose ? LogEventLevel.Information : LogEventLevel.Warning;
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: LogOutputTemplate)
                .WriteTo.File(
                    LogFileName,
                    outputTemplate: LogOutputTemplate,
                    rollingInterval: RollingInterval.Day)
                .MinimumLevel.Is(logEventLevel)
                .CreateLogger();   
        }
    }
}