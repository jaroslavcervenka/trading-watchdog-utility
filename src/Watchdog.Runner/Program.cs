using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MT5Wrapper;
using MT5Wrapper.Interface;
using Serilog;
using Watchdog.Worker;
using Watchdog.Worker.Interface;
using Watchdog.Worker.Model;
using ILogger = Serilog.ILogger;

namespace Watchdog.Runner
{
    internal class Program
    {
        private const string DefaultErrorMessage = "Unhandled Error Occurred. No details are known.";
        private const string SettingsFileName = "appsettings.json";
        private const string LogFileName = "wathdog.log";
        private const int SuccessExitCode = 0;
        private const int ErrorExitCode = -1;
        
        public static int Main(string[] args)
        {
            SetExceptionHandlers();
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(LogFileName)
                .CreateLogger();
            
            var result = Parser.Default
                            .ParseArguments<Options>(args)
                            .MapResult(RunApp, HandleParseError);
            return result;
        }

        private static int RunApp(Options options)
        {
            try
            {
                var services = ConfigureServices(options);
                var serviceProvider = services.BuildServiceProvider();
                serviceProvider.GetService<App>().Run();
                return SuccessExitCode;
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, $"Error occured: {e.Message}");
            }

            return ErrorExitCode;
        }

        private static IServiceCollection ConfigureServices(Options options)
        {
            IServiceCollection services = new ServiceCollection();
            var appOptions = new AppOptions(
                                options.Servers, 
                                options.Ratio, 
                                options.OpenTimeDelta);

            var config = LoadConfiguration();
            services.AddSingleton(config);
            services.AddSingleton(appOptions);
            services.AddLogging(configure =>
                configure.AddSerilog());

            // required to run the application
            services.AddSingleton<App>();
            services.AddSingleton<IMT5Api, MT5Api>();
            services.AddSingleton<IProducer, TestProducer>();

            return services;
        }
        
        private static IConfiguration LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(SettingsFileName, optional: true, reloadOnChange: true);

            return builder.Build();
        }
        
        private static int HandleParseError(IEnumerable<Error> errs)
        {
            var enumerable = errs as Error[] ?? errs.ToArray();
            Log.Logger.Error("Argument parse error orrured", errs);
            Console.WriteLine("errors {0}", enumerable.Count());
            return ErrorExitCode;
        }
        
        private static void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
        }
        
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                var message = DefaultErrorMessage;
                Exception exception = null;

                if (e.ExceptionObject != null && e.ExceptionObject is Exception uex)
                {
                    message = $"{DefaultErrorMessage} Exception: {uex.Message}";
                    exception = uex;
                }
                else if (sender is Exception ex)
                {
                    message = $"{DefaultErrorMessage} Exception: {ex.Message}";
                    exception = ex;
                }

                Log.Logger.Fatal(exception, $"Unhandled error occured: {message}");
            }
            catch(Exception ex)
            {
                Log.Logger.Error(ex, $"Unhandled error occured: {ex.Message}");
            }

            Environment.Exit(ErrorExitCode);
        }
    }
}