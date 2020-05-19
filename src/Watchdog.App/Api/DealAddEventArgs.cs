using System;
using Ardalis.GuardClauses;

namespace Watchdog.App.Api
{
    public class DealAddEventArgs<T> : EventArgs
    {
        public T Deal { get; }

        public DealAddEventArgs(T deal)
        {
            Guard.Against.Null(deal, nameof(deal));
            Deal = deal;
        }
    }
}