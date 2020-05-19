using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using MT5Wrapper;
using MT5Wrapper.Interface;
using Watchdog.App;
using Watchdog.App.Abstractions;
using Watchdog.App.Api;
using Watchdog.App.Comparer;
using Watchdog.App.Logger;
using Watchdog.App.Model;
using Watchdog.App.Producer;
using Watchdog.Worker.Core;

namespace Watchdog.Worker.Channels
{
    public static class Installer
    {
        public static void ConfigureServices(
            IServiceCollection services)
        {
            Guard.Against.Null(services, nameof(services));
            
            //Startup endpoint
            services.AddTransient<Startup>();
            
            //Instance ID
            services.AddSingleton(typeof(IInstanceIdGenerator<>), typeof(InstanceIdGenerator<>));
            
            //API
            services.AddSingleton<IMT5Api, MT5Api>();
            //services.AddTransient<IApi<Deal>, FakeApi>();
            services.AddTransient<IApi<Deal>, TradingApi>();
            
            //Comparer
            services.AddSingleton<IDealComparer<Deal>, DealComparer>();
            
            //Similar deals logger
            services.AddSingleton<ISimilarDealsLogger<Deal>, SimilarDealsLogger<Deal>>();
            
            //Queue - work items to compare
            services.AddSingleton<IWorkQueue<Deal>, WorkQueue<Deal>>();
            
            //Factories
            services.AddTransient<IApiProducerFactory, ApiProducerFactory>();
            services.AddTransient<IWorkConsumerFactory, WorkConsumerFactory>();
            services.AddTransient<IDealWatcherFactory, DealWatcherFactory>();
        }
    }
}