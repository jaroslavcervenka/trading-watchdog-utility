using System;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Watchdog.PerformanceTest.Helpers.AsyncProducer;
using Watchdog.App.Producer;

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