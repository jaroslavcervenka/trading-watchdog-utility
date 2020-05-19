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
    public class WorkConsumerFactory : IWorkConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public WorkConsumerFactory(IServiceProvider serviceProvider)
        {
            Guard.Against.Null(serviceProvider, nameof(serviceProvider));

            _serviceProvider = serviceProvider;
        }
        
        public IEnumerable<ValueTask> StartConsumers(
            Channel<Deal> apiChannel, 
            Channel<Deal> jobChannel, 
            int consumersCount,
            CancellationToken cancellationToken)
        {
            var consumerTasks = Enumerable.Range(1, consumersCount)
                .Select(i => CreateConsumer(apiChannel.Reader, jobChannel.Writer, i)
                    .BeginConsumeAsync(cancellationToken))
                .ToArray();

            foreach (var task in consumerTasks)
            {
                task.ConfigureAwait(false);
            }
            
            return consumerTasks;
        }

        private WorkConsumer<Deal> CreateConsumer(
            ChannelReader<Deal> apiReader,
            ChannelWriter<Deal> jobWriter,
            int instanceId
            )
        {
            var workQueue = _serviceProvider.GetService<IWorkQueue<Deal>>();
            var logger = _serviceProvider.GetService<ILogger<WorkConsumer<Deal>>>();
            var consumer = new WorkConsumer<Deal>(
                apiReader,
                jobWriter,
                workQueue,
                logger,
                instanceId);
            
            return consumer;
        }
    }
}