using Serilog;

namespace Watchdog.Logger
{
    internal static class Configure
    {
        private const string LogOutputTemplate = "{Timestamp:HH:mm:ss.fff} [{Level}] [{SourceContext}] [{ThreadId}] {Message}{NewLine}{Exception}";
        private const string LogFileName = "watchdog.log";
        
        internal static void CreateLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.With(new ThreadIdEnricher())
                .WriteTo.Console(outputTemplate: LogOutputTemplate)
                .WriteTo.File(
                    LogFileName,
                    outputTemplate: LogOutputTemplate,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();   
        }
    }
}