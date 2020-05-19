using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Moq;
using MT5Wrapper.Interface;
using Watchdog.App;
using Watchdog.App.Model;

namespace Watchdog.PerformanceTest
{
    // [SimpleJob(RunStrategy.ColdStart, targetCount: 1)]
    // [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    // public class ProducerTests
    // {
    //     [Benchmark]
    //     public void QueuePerformance()
    //     {
    //         var producer = CreateProducer();
    //         
    //         producer.
    //     }
    //
    //     private static IProducer CreateProducer(string ip)
    //     {
    //         var connectionConfig = new ConnectionConfig(
    //             ip, 
    //             1,
    //             "password");
    //         var api = CreateMT5ApiMock();
    //         var maxTimeDeltaMsc = 1000;
    //         var producer = new Producer(api, connectionConfig, maxTimeDeltaMsc);
    //
    //         return producer;
    //     }
    //
    //     private static IMT5Api CreateMT5ApiMock()
    //     {
    //         var apiMock = new Mock<IMT5Api>(MockBehavior.Strict);
    //
    //         return apiMock.Object;
    //     }
    // }
}