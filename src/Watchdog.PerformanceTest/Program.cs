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
            
        }

        // private const int DealsCount = 1_000_000;
        //
        // //static void Main() => BenchmarkRunner.Run<Program>(new AllowNonOptimized());
        // static void Main() => AsyncProducerConsumerQueue().Start();
        //
        // [Benchmark]
        // public static async Task AsyncProducerConsumerQueue()
        // {
        //     var cpuCount = Environment.ProcessorCount;
        //     
        //     var tokenSource = new CancellationTokenSource();
        //     const int expectedDealsCount = DealsCount * 2; 
        //     
        //     var producer1 = AsyncProducerConsumer.CreateProducer("1.1.1.1", DealsCount);
        //     var producer2 = AsyncProducerConsumer.CreateProducer("2.2.2.2", DealsCount);
        //     var consumer = AsyncProducerConsumer.CreateConsumer(
        //         expectedDealsCount, tokenSource);
        //
        //     await DoTestAsync(producer1, producer2, consumer, tokenSource.Token);
        //     // producer1.Subscribe(consumer.Consume, tokenSource.Token);
        //     // producer2.Subscribe(consumer.Consume, tokenSource.Token);
        //     //
        //     // await consumer.WaitForFinish(tokenSource.Token);
        // }
        //
        // private static async Task DoTestAsync(
        //     DealProducer producer1,
        //     DealProducer producer2,
        //     DealConsumer consumer,
        //     CancellationToken token)
        // {
        //     producer1.Subscribe(consumer.Consume, token);
        //     producer2.Subscribe(consumer.Consume, token);
        //     
        // }
    }
}