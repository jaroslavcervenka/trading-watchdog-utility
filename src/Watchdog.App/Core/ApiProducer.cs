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
            int instanceId,
            ILogger<ApiProducer<T>> logger)
        {
            Guard.Against.Null(api, nameof(api));
            Guard.Against.Null(apiWriter, nameof(apiWriter));
            Guard.Against.Null(connectionConfig, nameof(connectionConfig));
            Guard.Against.NegativeOrZero(instanceId, nameof(instanceId));
            Guard.Against.Null(logger, nameof(logger));

            _api = api;
            _apiWriter = apiWriter;
            _connectionConfig = connectionConfig;
            InstanceId = instanceId;
            _logger = logger;
        }

        public async ValueTask BeginProduceDealsFromApi(CancellationToken cancellationToken = default)
        {
            cancellationToken.Register(Disconnect);
            AddHandler();
            _api.Connect(_connectionConfig);

            while (!cancellationToken.IsCancellationRequested)
            {
                await _semaphoreSlim.WaitAsync(cancellationToken).ConfigureAwait(false);
                
                while (_queue.TryDequeue(out var deal))
                {
                    _logger.LogInformation($"{InstanceId}: read {deal.ToString()}");

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