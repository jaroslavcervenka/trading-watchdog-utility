using System.Collections.Generic;
using Watchdog.App.Abstractions;

namespace Watchdog.Worker.Core
{
    public class InstanceIdGenerator<T> : IInstanceIdGenerator<T>
    {
        private static Dictionary<string, int> _counters = new Dictionary<string, int>();
        private readonly object _locker = new object();
        
        public int GetNewId()
        {
            int instanceId;
            var type = typeof(T).Name;

            lock (_locker)
            {
                if (!_counters.ContainsKey(type))
                {
                    _counters.Add(type, 1);
                }
                instanceId = _counters[type]++;
            }

            return instanceId;
        }
    }
}