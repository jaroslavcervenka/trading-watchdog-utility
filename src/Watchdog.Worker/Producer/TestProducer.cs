using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Logging;
using MT5Wrapper;
using MT5Wrapper.Interface;
using Watchdog.Worker.Interface;
using Watchdog.Worker.Model;

namespace Watchdog.Worker
{
    public class TestProducer : IProducer
    {
        private readonly IMT5Api _mt5Api;
        private readonly ConnectionConfig _connectionConfig;
        private readonly long _maxTimeDeltaMsc;
        private readonly ILogger<TestProducer> _logger;

        public BlockingCollection<Deal> Queue { get; }

        public TestProducer(
            IMT5Api mt5Api,
            ConnectionConfig connectionConfig,
            long maxTimeDeltaMsc,
            ILogger<TestProducer> logger)
        {
            Guard.Against.Null(mt5Api, nameof(mt5Api));
            Guard.Against.Null(connectionConfig, nameof(connectionConfig));
            Guard.Against.NegativeOrZero(maxTimeDeltaMsc, nameof(maxTimeDeltaMsc));
            Guard.Against.Null(logger, nameof(logger));
            
            _mt5Api = mt5Api;
            _connectionConfig = connectionConfig;
            _maxTimeDeltaMsc = maxTimeDeltaMsc;
            _logger = logger;
            Queue = new BlockingCollection<Deal>();
        }

        public Task WaitDealEventsAsync(CancellationToken token)
        {
            token.Register(UnsubscribeDealEvents);
            return Task.Run(WaitDealEventsInternal, token);
        }

        private void WaitDealEventsInternal()
        {
            Connect();
            SubscribeDealEvents();
        }

        private void Connect()
        {
            var connectionParams = new ConnectionParams
            {
                IP = _connectionConfig.Ip,
                Login = _connectionConfig.Login,
                Password = _connectionConfig.Password
            };
            _mt5Api.Connect(connectionParams);
        }

        private void Disconnect()
        {
            _mt5Api.Disconnect();
        }

        private void SubscribeDealEvents()
        {
            _mt5Api.DealEvents.DealAddEventHandler += DealEventsOnDealAddEventHandler;
        }

        private void UnsubscribeDealEvents()
        {
            _mt5Api.DealEvents.DealAddEventHandler -= DealEventsOnDealAddEventHandler;
            Disconnect();
        }

        private void DealEventsOnDealAddEventHandler(object control, CIMTDeal mtDeal)
        {
            var dealAction = (CIMTDeal.EnDealAction)mtDeal.Action();

            if (dealAction != CIMTDeal.EnDealAction.DEAL_BUY
                || dealAction != CIMTDeal.EnDealAction.DEAL_SELL)
            {
                return;
            }
            
            var login = mtDeal.Login();
            var sellAction = dealAction != CIMTDeal.EnDealAction.DEAL_SELL;
            var volume = mtDeal.Volume();
            //copy string into new memory allocation
            var symbol = mtDeal.Symbol();
            var newSymbolInstance = new string(symbol.ToCharArray());
            var timeMsc = mtDeal.TimeMsc();
            var deal = new Deal(
                login,
                sellAction,
                volume,
                newSymbolInstance,
                timeMsc
                );
            
            Queue.Add(deal);
            //RemoveOldestDeals(timeMsc);
            Queue.CompleteAdding();
            
            _logger.LogInformation($"{_connectionConfig.Ip}:{sellAction}, ");
        }

        // private void RemoveOldestDeals(long timeMsc)
        // {
        //     Queue.
        // }
    }
}