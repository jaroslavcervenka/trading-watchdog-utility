using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;
using Watchdog.App.Api;
using Watchdog.App.Model;

namespace Watchdog.Worker.Core
{
    public class ApiProducer<T> : IInstance
    {
        public int InstanceId { get; }
        
        private readonly IApi<T> _api;
        private readonly ChannelWriter<T> _apiWriter;
        private readonly ConnectionConfig _connectionConfig;
        private readonly ILogger<ApiProducer<T>> _logger;
        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(0);
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();

        public ApiProducer(
            ChannelWriter<T> apiWriter,
            IApi<T> api,
            ConnectionConfig connectionConfig,
            ILogger<ApiProducer<T>> logger,
            IInstanceIdGenerator<ApiProducer<T>> instanceIdGenerator
        )
        {
            Guard.Against.Null(api, nameof(api));
            Guard.Against.Null(apiWriter, nameof(apiWriter));
            Guard.Against.Null(connectionConfig, nameof(connectionConfig));
            Guard.Against.Null(logger, nameof(logger));
            Guard.Against.Null(instanceIdGenerator, nameof(instanceIdGenerator));
            
            _api = api;
            _apiWriter = apiWriter;
            _connectionConfig = connectionConfig;
            _logger = logger;
            InstanceId = instanceIdGenerator.GetNewId();
        }

        public async ValueTask BeginProduceDealsFromApi(CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            
            AddHandler();
            _api.Connect(_connectionConfig);

            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Disconnect();
                    return;
                }

                await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
                
                while (_queue.TryDequeue(out var deal))
                {
                    if (deal == null)
                    {
                        _logger.LogError("Getting null deal value.");
                        continue;
                    }
                    
                    await _apiWriter.WriteAsync(deal, cancellationToken).ConfigureAwait(false);
                }
                
            }
        }

        private void AddHandler()
        {
            _api.DealAdded += OnDealAdded;
        }
        
        private void RemoveHandler()
        {
            _api.DealAdded -= OnDealAdded;
        }

        private void OnDealAdded(object sender, DealAddEventArgs<T> e)
        {
            _queue.Enqueue(e.Deal);
            PerformanceCounter.PerfCounters.SingleInstance.IncomingDealsQueueCount.RegisterValue(_queue.Count);
            _semaphoreSlim.Release();
        }
        
        private void Disconnect()
        {
            RemoveHandler();
            _api.Disconnect();
        }
    }
}