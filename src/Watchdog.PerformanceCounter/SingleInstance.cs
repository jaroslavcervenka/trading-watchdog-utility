using Qoollo.PerformanceCounters;

namespace Watchdog.PerformanceCounter
{
    public class SingleInstance: SingleInstanceCategoryWrapper
    {
        public SingleInstance() 
            : base("Watchdog", "Counters for Watchdog") { }

        [Counter("Deal events from API to process", "Incoming deals queue counter")]
        public AverageCountCounter IncomingDealsQueueCount { get; private set; }

        [Counter("Deals to compare", "Work queue counter")]
        public AverageCountCounter WorkDealsQueueCount { get; private set; }
    }
}