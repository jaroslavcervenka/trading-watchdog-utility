using System;
using Ardalis.GuardClauses;
using Watchdog.App.Abstractions;
using Watchdog.App.Model;

namespace Watchdog.App.Comparer
{
    public class DealComparer : IDealComparer<Deal>
    {
        private const int TimeKeepInQueueSeconds = 2;
        
        private readonly int _maxOpenTimeDelta;
        private readonly decimal _maxTradeVolumeToBalanceRatio;

        public DealComparer(AppOptions appOptions)
        {
            Guard.Against.Null(appOptions, nameof(appOptions));

            _maxOpenTimeDelta = appOptions.OpenTimeDelta;
            _maxTradeVolumeToBalanceRatio = appOptions.Ratio;
        }
        
        public bool Compare(Deal dealA, Deal dealB)
        {
            return 
                   !dealA.Equals(dealB)
                   && dealA.SellAction == dealB.SellAction
                   && dealA.Symbol == dealB.Symbol
                   && dealA.VolumeToBalanceRatio <= _maxTradeVolumeToBalanceRatio
                   && dealB.VolumeToBalanceRatio <= _maxTradeVolumeToBalanceRatio
                   && Math.Abs(dealA.TimeMsc - dealB.TimeMsc) <= _maxOpenTimeDelta;
        }

        public bool IsOlderThanLimit(Deal latestDeal, Deal previousDeal)
        {
            return latestDeal.TimeMsc - previousDeal.TimeMsc > _maxOpenTimeDelta;
        }

        public bool CanBeRemoved(Deal latestDeal, Deal previousDeal)
        {
            return latestDeal.TimeMsc - previousDeal.TimeMsc - TimeKeepInQueueSeconds > 1;
        }
    }
}