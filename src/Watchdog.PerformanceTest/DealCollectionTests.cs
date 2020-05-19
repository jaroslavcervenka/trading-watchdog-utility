using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Watchdog.Worker.Model;

namespace Watchdog.PerformanceTest
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 1)]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class DealCollectionTests
    {
        [Benchmark]
        public void HundredDeals_RemoveAllNotValid()
        {
            var queue = new Queue<Deal>();

            for (int i = 0; i < 300000; i++)
            {
                var deal = new Deal(
                    1212313,
                    true,
                    2100,
                    "EURUSD",
                    DateTimeOffset.Now.ToUnixTimeMilliseconds()
                );
            }
        }
    }
}