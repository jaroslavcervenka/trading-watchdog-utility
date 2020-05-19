using Qoollo.PerformanceCounters;

namespace Watchdog.PerformanceCounter
{
    public class PerfCounters : Qoollo.PerformanceCounters.PerfCountersContainer
    {
        public static SingleInstance SingleInstance { get; private set; } = CreateNullCategoryWrapper<SingleInstance>();

        public static void Init(CategoryWrapper parent)
        {
            SingleInstance = parent.CreateSubCategory<SingleInstance>();
        }
    }
}