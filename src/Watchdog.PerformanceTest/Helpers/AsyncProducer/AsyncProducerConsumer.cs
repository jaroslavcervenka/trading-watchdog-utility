// using System.Threading;
// using Watchdog.App.Abstractions;
// using Watchdog.App.Model;
// using Watchdog.App.Producer;
// using Watchdog.Worker.Queue;
//
// namespace Watchdog.PerformanceTest.Helpers.AsyncProducer
// {
//     internal static class AsyncProducerConsumer
//     {
//         internal static DealProducer CreateProducer(
//             string ip,
//             int dealsCount)
//         {
//             var connectionConfig = new ConnectionConfig(
//                 ip, 
//                 1,
//                 "password");
//             var api = CreateApi(dealsCount);
//             var jobQueue = CreateQueue();
//             var logger = new NullLogger<DealProducer>();
//             
//             var producer = new DealProducer(
//                 api, 
//                 connectionConfig, 
//                 jobQueue, 
//                 logger);
//
//             return producer;
//         }
//         
//         internal static DealConsumer CreateConsumer(
//             int expectedDealsCount,
//             CancellationTokenSource tokenSource)
//         {
//             var logger = new NullLogger<DealConsumer>();
//             var consumer = new DealConsumer(
//                 null,
//                 null,
//                 null,
//                 logger);
//
//             return consumer;
//         }
//         
//         private static IJobQueue<Deal> CreateQueue()
//         {
//             var logger = new NullLogger<AsyncProducerConsumerQueue<Deal>>();
//             var queue = new AsyncProducerConsumerQueue<Deal>(
//                 logger);
//
//             return queue;
//         }
//         
//         internal static IApi CreateApi(int dealsCount)
//         {
//             var logger = new NullLogger<TestApi>();
//             return new TestApi(logger, dealsCount);
//         }
//     }
// }// using System.Threading;
// using Watchdog.App.Abstractions;
// using Watchdog.App.Model;
// using Watchdog.App.Producer;
// using Watchdog.Worker.Queue;
//
// namespace Watchdog.PerformanceTest.Helpers.AsyncProducer
// {
//     internal static class AsyncProducerConsumer
//     {
//         internal static DealProducer CreateProducer(
//             string ip,
//             int dealsCount)
//         {
//             var connectionConfig = new ConnectionConfig(
//                 ip, 
//                 1,
//                 "password");
//             var api = CreateApi(dealsCount);
//             var jobQueue = CreateQueue();
//             var logger = new NullLogger<DealProducer>();
//             
//             var producer = new DealProducer(
//                 api, 
//                 connectionConfig, 
//                 jobQueue, 
//                 logger);
//
//             return producer;
//         }
//         
//         internal static DealConsumer CreateConsumer(
//             int expectedDealsCount,
//             CancellationTokenSource tokenSource)
//         {
//             var logger = new NullLogger<DealConsumer>();
//             var consumer = new DealConsumer(
//                 null,
//                 null,
//                 null,
//                 logger);
//
//             return consumer;
//         }
//         
//         private static IJobQueue<Deal> CreateQueue()
//         {
//             var logger = new NullLogger<AsyncProducerConsumerQueue<Deal>>();
//             var queue = new AsyncProducerConsumerQueue<Deal>(
//                 logger);
//
//             return queue;
//         }
//         
//         internal static IApi CreateApi(int dealsCount)
//         {
//             var logger = new NullLogger<TestApi>();
//             return new TestApi(logger, dealsCount);
//         }
//     }
// }