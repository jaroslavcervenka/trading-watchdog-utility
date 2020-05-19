using System;
using System.Threading;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.Worker.Api;
using Watchdog.Worker.Interface;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Producer
{
    public class DealProducer
    {
        private readonly IApi _api;
        private readonly ConnectionConfig _connectionConfig;
        private readonly ILogger<DealProducer> _logger;
        private readonly IJobQueue<Deal> _jobQueue;
        private readonly object _locker = new object();

        private int _counter;

        public DealProducer(
            IApi api,
            ConnectionConfig connectionConfig,
            IJobQueue<Deal> jobQueue,
            ILogger<DealProducer> logger)
        {
            Guard.Against.Null(api, nameof(api));
            Guard.Against.Null(connectionConfig, nameof(connectionConfig));
            Guard.Against.Null(jobQueue, nameof(jobQueue));
            Guard.Against.Null(logger, nameof(logger));
            
            _api = api;
            _connectionConfig = connectionConfig;
            _jobQueue = jobQueue;
            _logger = logger;
        }

        public void Subscribe(
            Action<Deal> consumer,
            CancellationToken token)
        {
            Guard.Against.Null(consumer, nameof(consumer));
            Guard.Against.Null(token, nameof(token));

            if (token.IsCancellationRequested)
            {
                return;
            }

            token.Register(Disconnect);
            AddConsumer(consumer, token);
            AddHandler();
            Connect();
        }

        private void AddConsumer(
            Action<Deal> consumer,
            CancellationToken token)
        {
            _jobQueue.AddConsumer(consumer, token);
        }

        private void AddHandler()
        {
            _api.DealAdded += OnDealAdded;
        }

        private void RemoveHandler()
        {
            _api.DealAdded -= OnDealAdded;
        }

        private void OnDealAdded(object sender, DealAddEventArgs e)
        {
            Interlocked.Increment(ref _counter);
            _jobQueue.Produce(e.Deal);
        }

        private void Disconnect()
        {
            RemoveHandler();
            _api.Disconnect();
            _logger.LogInformation($"Disconnected from server {_connectionConfig.Ip} (counter: {_counter})");
        }

        private void Connect()
        {
            _api.Connect(_connectionConfig);
            _logger.LogInformation($"Connected to server {_connectionConfig.Ip}");
        }
    }
}