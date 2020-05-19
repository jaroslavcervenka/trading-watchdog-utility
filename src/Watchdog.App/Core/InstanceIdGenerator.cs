using Watchdog.App.Abstractions;

namespace Watchdog.Worker.Core
{
    public class InstanceIdGenerator<T> : IInstanceIdGenerator<T>
    {
        private static int counter = 1;
        
        public int GetNewId()
        {
            return counter++;
        }
    }
}