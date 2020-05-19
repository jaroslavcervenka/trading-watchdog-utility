using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Watchdog.App.Model;

namespace Watchdog.App.Abstractions
{
    public interface IDealWatcherFactory
    {
        IEnumerable<Task> StartWatchers(
            Channel<Deal> jobChannel,
            int watchersCount,
            CancellationToken cancellationToken);
    }
}