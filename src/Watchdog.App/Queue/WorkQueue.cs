using System.Collections.Concurrent;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;
using Watchdog.PerformanceCounter;

namespace Watchdog.App.Producer
{
    public class WorkQueue<T> : IWorkQueue<T> where T : struct
    {
        public int Count => _queue.Count;
        
        private readonly ConcurrentQueue<T> _queue;
        private readonly IDealComparer<T> _comparer;
        private readonly ILogger<WorkQueue<T>> _logger;
        private readonly object _locker = new object();

        public WorkQueue(
            IDealComparer<T> comparer,
            ILogger<WorkQueue<T>> logger)
        {
            Guard.Against.Null(comparer, nameof(comparer));
            Guard.Against.Null(logger, nameof(logger));

            _comparer = comparer;
            _logger = logger;
            _queue = new ConcurrentQueue<T>();
        }

        public void Enqueue(T item)
        {
            Guard.Against.Null(item, nameof(item));
            
            _queue.Enqueue(item);
        }

        public bool TryDequeue(out T result)
        {
            return _queue.TryDequeue(out result);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        public void Cleanup(T latestDeal)
        {
            lock (_locker)
            {
                CleanupInternal(latestDeal);
            }
            PerfCounters.SingleInstance.WorkDealsQueueCount.RegisterValue(_queue.Count);
        }

        private void CleanupInternal(T latestDeal)
        {
            var itemsToRemove = 0;
            
            foreach (var deal in _queue)
            {
                if (_comparer.CanBeRemoved(latestDeal, deal))
                {
                    itemsToRemove++;    
                }
                else
                {
                    break;
                }
            }

            for (var i = 0; i < itemsToRemove; i++)
            {
                T dealresult;
                var result = _queue.TryDequeue(out dealresult);
                
                if (!result)
                {
                    _logger.LogWarning("Cannot remove item from work queue.");
                }
            }
        }
    }
}