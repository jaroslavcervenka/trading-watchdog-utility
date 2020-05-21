using BenchmarkDotNet.Attributes;

namespace Watchdog.PerformanceTest
{
    [MemoryDiagnoser]
    [SimpleJob(targetCount: 3)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class Program
    {
        static void Main()
        {
            //TODO: write some tests   
        }
    }
}