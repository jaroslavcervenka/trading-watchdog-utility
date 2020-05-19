using System;
using System.Threading;

namespace Watchdog.Worker.Interface
{
    public interface IJobQueue<T>
    {

        public void AddConsumer(
            Action<T> consumer,
            CancellationToken token);
        
        public void Produce(T value);
    }
}