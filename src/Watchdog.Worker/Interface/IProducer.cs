using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Interface
{
    public interface IProducer
    {
        public BlockingCollection<Deal> Queue { get; }

        public Task WaitDealEventsAsync(CancellationToken token);
    }
}