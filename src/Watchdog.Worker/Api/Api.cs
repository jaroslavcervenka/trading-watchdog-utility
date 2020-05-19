using System;
using Ardalis.GuardClauses;
using FluentResults;
using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.Logging;
using MT5Wrapper;
using MT5Wrapper.Interface;
using Watchdog.Worker.Interface;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Api
{
    public class Api : IApi, IDisposable
    {
        private readonly ILogger<Api> _logger;

        private IMT5Api _mt5Api;
        
        public event EventHandler<DealAddEventArgs> DealAdded = delegate { };
        
        public Api(
            IMT5Api mt5Api,
            ILogger<Api> logger)
        {
            Guard.Against.Null(mt5Api, nameof(mt5Api));
            Guard.Against.Null(logger, nameof(logger));

            _mt5Api = mt5Api;
            _logger = logger;
        }

        public void Connect(ConnectionConfig connectionConfig)
        {
            var connectionParams = new ConnectionParams
            {
                IP = connectionConfig.Ip,
                Login = connectionConfig.Login,
                Password = connectionConfig.Password
            };
            _mt5Api.Connect(connectionParams);
            _mt5Api.DealEvents.DealAddEventHandler += DealEventsOnDealAddEventHandler;
            _logger.LogInformation($"Connected to server {connectionConfig.Ip}");
        }

        public void Disconnect()
        {
            DealAdded = delegate { };
            _mt5Api.DealEvents.DealAddEventHandler -= DealEventsOnDealAddEventHandler;
            _mt5Api.Disconnect();
        }

        public Result<decimal> GetUserBalance(ulong login)
        {
            try
            {
                var balance = _mt5Api.GetUserBalance(login);
                return Results.Ok(balance);
            }
            catch (MT5Exception ex)
            {
                var errorMessage = $"Error occured during getting user balance: (login: {login})"; 
                _logger.LogError(errorMessage);
                return Results.Fail(errorMessage);
            }
        }

        public void Dispose()
        {
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
            
            DealAdded.Invoke(control, new DealAddEventArgs(deal));
        }
    }
}