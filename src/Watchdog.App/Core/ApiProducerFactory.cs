using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;
using Watchdog.App.Model;

namespace Watchdog.Worker.Core
{
    public class ApiProducerFactory : IApiProducerFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AppOptions _appOptions;
        
        public ApiProducerFactory(
            IServiceProvider serviceProvider,
            AppOptions appOptions)
        {
            Guard.Against.Null(serviceProvider, nameof(serviceProvider));
            Guard.Against.Null(appOptions, nameof(appOptions));
            
            _serviceProvider = serviceProvider;
            _appOptions = appOptions;
        }

        public IEnumerable<Task> StartProducers(
            Channel<Deal> apiChannel,
            CancellationToken cancellationToken)
        {
            var producerTasks = _appOptions
                .Servers
                .Select(serverIp => CreateProducer(serverIp, apiChannel.Writer)
                    .BeginProduceDealsFromApi(cancellationToken).AsTask())
                .ToArray();
            
            foreach (var task in producerTasks)
            {
                task.ConfigureAwait(false);
            }

            return producerTasks;
        }

        private ApiProducer<Deal> CreateProducer(
            string serverIp,
            ChannelWriter<Deal> apiWriter)
        {
            var connectionConfig = new ConnectionConfig(
                serverIp, 
                _appOptions.Login, 
                _appOptions.Password);
            var api = _serviceProvider.GetService<IApi<Deal>>();
            var logger = _serviceProvider.GetService<ILogger<ApiProducer<Deal>>>();
            var instanceIdGenerator = _serviceProvider.GetService<IInstanceIdGenerator<ApiProducer<Deal>>>();
            var producer = new ApiProducer<Deal>(
                apiWriter,
                api,
                connectionConfig,
                logger,
                instanceIdGenerator);

            return producer;
        }
    }
}