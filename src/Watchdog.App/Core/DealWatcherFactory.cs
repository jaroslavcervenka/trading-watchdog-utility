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
    public class DealWatcherFactory : IDealWatcherFactory
    {
        private readonly IServiceProvider _serviceProvider;
        
        public DealWatcherFactory(IServiceProvider serviceProvider)
        {
            Guard.Against.Null(serviceProvider, nameof(serviceProvider));
            
            _serviceProvider = serviceProvider;
        }
        
        public IEnumerable<Task> StartWatchers(
            Channel<Deal> jobChannel, 
            int watchersCount, 
            CancellationToken cancellationToken)
        {
            var watcherTasks = Enumerable.Range(1, watchersCount)
                .Select(i => 
                    CreateWatcher(jobChannel.Reader).BeginConsumeAsync(cancellationToken).AsTask()
                )
                .ToArray();

            foreach (var task in watcherTasks)
            {
                task.ConfigureAwait(false);
            }
            
            return watcherTasks;
        }

        private DealWatcher<Deal> CreateWatcher(ChannelReader<Deal> jobReader)
        { 
            var comparer = _serviceProvider.GetService<IDealComparer<Deal>>();
            var workQueue = _serviceProvider.GetService<IWorkQueue<Deal>>();
            var similarDealsLogger = _serviceProvider.GetService<ISimilarDealsLogger<Deal>>();
            var logger = _serviceProvider.GetService<ILogger<DealWatcher<Deal>>>();
            var instanceIdGenerator = _serviceProvider.GetService<IInstanceIdGenerator<DealWatcher<Deal>>>();
            var watcher = new DealWatcher<Deal>(
                comparer,
                workQueue,
                jobReader,
                similarDealsLogger,
                logger,
                instanceIdGenerator);

            return watcher;
        }
    }
}