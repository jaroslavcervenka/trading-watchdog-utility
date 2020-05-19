using System;
using System.Threading.Tasks;
using FluentResults;
using Watchdog.App.Api;
using Watchdog.App.Model;

namespace Watchdog.App.Abstractions
{
    public interface IApi<T>
    {
        event EventHandler<DealAddEventArgs<T>> DealAdded;
        void Connect(ConnectionConfig connectionConfig);

        void Disconnect();

        Result<decimal> GetUserBalance(ulong login);
    }
}