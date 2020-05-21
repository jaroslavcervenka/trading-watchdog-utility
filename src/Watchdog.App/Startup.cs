using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Watchdog.App.Abstractions;
using Watchdog.App.Model;

namespace Watchdog.App
{
    public class Startup
    {
        private readonly IApiProducerFactory _producerFactory;
        private readonly IWorkConsumerFactory _consumerFactory;
        private readonly IDealWatcherFactory _watcherFactory;
        
        private const int ConsumersCount = 1;
        private const int WatchersCount = 2;
        
        public Startup(
            IApiProducerFactory producerFactory,
            IWorkConsumerFactory consumerFactory,
            IDealWatcherFactory watcherFactory)
        {
            Guard.Against.Null(producerFactory, nameof(producerFactory));
            Guard.Against.Null(consumerFactory, nameof(consumerFactory));
            Guard.Against.Null(watcherFactory, nameof(watcherFactory));
            
            _producerFactory = producerFactory;
            _consumerFactory = consumerFactory;
            _watcherFactory = watcherFactory;
        }

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            var apiChannel = Channel.CreateUnbounded<Deal>();
            var jobChannel = Channel.CreateUnbounded<Deal>();
            var tasks = new List<Task>(_watcherFactory.StartWatchers(
                                                            jobChannel, 
                                                            WatchersCount,
                                                            cancellationToken));
            tasks.AddRange(_consumerFactory.StartConsumers(
                                                            apiChannel, 
                                                            jobChannel, 
                                                            ConsumersCount, 
                                                            cancellationToken));
            tasks.AddRange(_producerFactory.StartProducers(apiChannel, cancellationToken));
            
            await Task.WhenAll(tasks);
        }
    }
}