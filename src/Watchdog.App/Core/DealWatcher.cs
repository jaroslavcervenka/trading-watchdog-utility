using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;

namespace Watchdog.Worker.Core
{
    public class DealWatcher<T> : IInstance where T : struct
    {
        public int InstanceId { get; }

        private readonly IDealComparer<T> _comparer;
        private readonly IWorkQueue<T> _workQueue;
        private readonly ChannelReader<T> _jobReader;
        private readonly ISimilarDealsLogger<T> _similarDealsLogger;
        private readonly ILogger<DealWatcher<T>> _logger;

        public DealWatcher(
            IDealComparer<T> comparer,
            IWorkQueue<T> workQueue,
            ChannelReader<T> jobReader,
            ISimilarDealsLogger<T> similarDealsLogger,
            ILogger<DealWatcher<T>> logger,
            IInstanceIdGenerator<DealWatcher<T>> instanceIdGenerator)
        {
            Guard.Against.Null(comparer, nameof(comparer));
            Guard.Against.Null(workQueue, nameof(workQueue));
            Guard.Against.Null(jobReader, nameof(jobReader));
            Guard.Against.Null(similarDealsLogger, nameof(similarDealsLogger));
            Guard.Against.Null(logger, nameof(logger));
            Guard.Against.Null(instanceIdGenerator, nameof(instanceIdGenerator));

            _comparer = comparer;
            _workQueue = workQueue;
            _jobReader = jobReader;
            _similarDealsLogger = similarDealsLogger;
            _logger = logger;
            InstanceId = instanceIdGenerator.GetNewId();
        }

        public async ValueTask BeginConsumeAsync(CancellationToken cancellationToken = default)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var deal = await _jobReader.ReadAsync(cancellationToken).ConfigureAwait(false);
                
                DetectSimilarDeals(deal);
                _workQueue.Enqueue(deal);
            }
        }
        
        private void DetectSimilarDeals(T newDeal)
        {
            var similarDeals = new List<T>();
            
            foreach (var deal in _workQueue)
            {
                if (_comparer.Compare(newDeal, deal))
                {
                    similarDeals.Add(deal);
                }
            }

            if (similarDeals.Any())
            {
                _similarDealsLogger.Log(newDeal, similarDeals);
            }
        }
    }
}