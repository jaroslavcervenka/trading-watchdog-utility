using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Watchdog.App.Model;

namespace Watchdog.App.Abstractions
{
    public interface IWorkConsumerFactory
    {
        IEnumerable<ValueTask> StartConsumers(
            Channel<Deal> apiChannel,
            Channel<Deal> jobChannel,
            int consumersCount,
            CancellationToken cancellationToken);
    }
}