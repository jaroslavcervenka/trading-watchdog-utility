using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Watchdog.App.Model;

namespace Watchdog.App.Abstractions
{
    public interface IApiProducerFactory
    {
        IEnumerable<Task> StartProducers(
            Channel<Deal> apiChannel,
            CancellationToken cancellationToken);
    }
}