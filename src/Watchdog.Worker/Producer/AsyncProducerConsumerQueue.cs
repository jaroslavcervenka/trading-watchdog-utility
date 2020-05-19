using System;
using System.Collections.Concurrent;
using System.Threading;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.Worker.Interface;

namespace Watchdog.Worker.Producer
{
    public class AsyncProducerConsumerQueue<T> : IJobQueue<T>, IDisposable
    {
        private readonly BlockingCollection<T> _queue;
        private readonly ILogger<AsyncProducerConsumerQueue<T>> _logger;
     private readonly object _locker = new object();
        
        
        private Action<T> _consumer;
        private bool _isDisposed;

        private int _counter;

        public AsyncProducerConsumerQueue(
            ILogger<AsyncProducerConsumerQueue<T>> logger)
        {
            Guard.Against.Null(logger, nameof(logger));

            _consumer = NullConsumer<T>.Consume;
            _logger = logger;
            _queue = new BlockingCollection<T>(new ConcurrentQueue<T>());
        }

        public void AddConsumer(
            Action<T> consumer,
            CancellationToken token)
        {
            Guard.Against.Null(consumer, nameof(consumer));
            Guard.Against.Null(token, nameof(token));
            
            _consumer = consumer;
            new Thread(() => ConsumeLoop(token)).Start();
        }

        public void Produce(T value)
        {
            _queue.Add(value);
            Interlocked.Increment(ref _counter);
        }
        
        private void ConsumeLoop(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                try
                {
                    var item = _queue.Take(cancelToken);
                    _consumer(item);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation($"Consume of queue cancelled. (counter: {_counter})");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occured when taking item from queue.");
                    throw;
                }
            }
        }
         
        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    _queue.Dispose();
                }

                _isDisposed = true;
            }
        }
    
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}