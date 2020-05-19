using System;
using Ardalis.GuardClauses;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Api
{
    public class DealAddEventArgs : EventArgs
    {
        public Deal Deal { get; }

        public DealAddEventArgs(Deal deal)
        {
            Guard.Against.Null(deal, nameof(deal));
            Deal = deal;
        }
    }
}