using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using FluentResults;
using Microsoft.Extensions.Logging;
using ParkSquare.Testing.Generators;
using Watchdog.Utils;
using Watchdog.Utils.MicroLibrary;
using Watchdog.App.Abstractions;
using Watchdog.App.Model;

namespace Watchdog.App.Api
{
    public class FakeApi : IApi<Deal>, IInstance
    {
        public event EventHandler<DealAddEventArgs<Deal>> DealAdded = delegate {  };
        
        public int InstanceId { get; }

        private const int Wait = 2;

        private readonly ILogger<FakeApi> _logger;
        private readonly MicroTimer _timer;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
        private volatile bool _started;

        private readonly Random _random = new Random();

        public FakeApi(
            IInstanceIdGenerator<IApi<Deal>> instanceIdGenerator,
            ILogger<FakeApi> logger)
        {
            Guard.Against.Null(logger, nameof(logger));

            _timer = new MicroTimer(Wait);
            _logger = logger;
            
            InstanceId = instanceIdGenerator.GetNewId();
        }

        public void Connect(ConnectionConfig connectionConfig)
        {
            _logger.LogInformation($"API connected to: {connectionConfig.Ip}");
            SetTimer();
            Task.Run(() =>
            { 
                _started = true;
                
                while (_started)
                {
                    NewDealAddedEvent();
                    _semaphore.Wait();
                }
            });
        }

        private void NewDealAddedEvent()
        {
            var deal = new Deal(
                (ulong) _random.Next(),
                BooleanGenerator.IsCoinTossHeads(),
                (ulong) _random.Next(),
                DecimalGenerator.AnyPositiveCurrencyAmount(),
                StringGenerator.SequenceOfAlphas(6),
                CurrentUnixTimeSeconds());

            DealAdded.Invoke(this, new DealAddEventArgs<Deal>(deal));
            //_logger.LogInformation($"API {InstanceId} {deal.ToString()}");
        }

        public void Disconnect()
        {
            _timer.Enabled = false;
            _started = false;
        }

        public Result<decimal> GetUserBalance(ulong login)
        {
            const decimal balance = 10000;
            return Results.Ok(balance);
        }

        private static long CurrentUnixTimeSeconds()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.Now);
            return dateTimeOffset.ToUnixTimeSeconds();
        }

        private void SetTimer()
        {
            _timer.MicroTimerElapsed += OnTimedEvent;
            _timer.Enabled = true;
        }

        private void OnTimedEvent(object sender,
            MicroTimerEventArgs timerEventArgs)
        {
            _semaphore.Release();
        }
    }
}