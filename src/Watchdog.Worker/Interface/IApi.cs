using System;
using FluentResults;
using Watchdog.Worker.Api;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Interface
{
    public interface IApi
    {
        event EventHandler<DealAddEventArgs> DealAdded;
        void Connect(ConnectionConfig connectionConfig);

        void Disconnect();

        Result<decimal> GetUserBalance(ulong login);
    }
}