using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Watchdog.App;
using Watchdog.PerformanceCounter;
using Watchdog.App.Model;
using Watchdog.Worker.Channels;

namespace Watchdog
{
    public static class Program
    {
        private const string DefaultErrorMessage = "Unhandled Error Occurred. No details are known.";
        private const string SettingsFileName = "appsettings.json";
        private const int SuccessExitCode = 0;
        private const int ErrorExitCode = -1;


        public static async Task<int> Main(string[] args)
        {
            SetExceptionHandlers();
            return await RunAsync(args);
        }

        private static async ValueTask<int> RunAsync(IEnumerable<string> args)
        {
            var result = await Parser.Default
                 .ParseArguments<Options>(args)
                 .MapResult(RunAppAsync, HandleParseError);
             
            return result;
        }

        private static async Task<int> RunAppAsync(Options options)
        {
            Logger.Configure.CreateLogger(options.Verbose);
            
            Factory? performanceCountersFactory = null;
            var tokenSource = new CancellationTokenSource();
            try
            {
                performanceCountersFactory = InitPerformanceCounters();
                var services = ConfigureServices(options);
                var serviceProvider = services.BuildServiceProvider();
                var startup = serviceProvider.GetService<Startup>();

                await startup.RunAsync(tokenSource.Token);

                return SuccessExitCode;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error occured: {e.Message}");
            }
            finally
            {
                tokenSource.Cancel();
                performanceCountersFactory?.Dispose();
            }

            return ErrorExitCode;
        }

        private static Factory InitPerformanceCounters()
        {
            var performanceCounterFactory = new Factory();
            performanceCounterFactory.Init();

            return performanceCounterFactory;
        }

        private static IServiceCollection ConfigureServices(Options options)
        {
            IServiceCollection services = new ServiceCollection();
            var appOptions = new AppOptions(
                                options.Servers, 
                                options.Login,
                                options.Password,
                                options.Ratio, 
                                options.OpenTimeDelta);

            var config = LoadConfiguration();
            services.AddSingleton(config);
            services.AddSingleton(appOptions);
            services.AddLogging(configure =>
                configure.AddSerilog());

            Installer.ConfigureServices(services);

            return services;
        }
        
        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: true);

            return builder.Build();
        }
        
        private static Task<int> HandleParseError(IEnumerable<Error> errs)
        {
            var enumerable = errs as Error[] ?? errs.ToArray();
            Log.Logger.Error("Argument parse error orrured", errs);
            Console.WriteLine("errors {0}", enumerable.Count());
            return Task.FromResult(ErrorExitCode);
        }
        
        private static void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }
        
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var exception = new Exception(DefaultErrorMessage);

                if (e.ExceptionObject != null && e.ExceptionObject is Exception uex)
                {
                    exception = uex;
                }
                else if (sender is Exception ex)
                {
                    exception = ex;
                }

                Log.Logger.Fatal(exception, exception.Message);
            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex, $"Unhandled error occured: {ex.Message}");
            }

            Environment.Exit(ErrorExitCode);
        }
    }
}