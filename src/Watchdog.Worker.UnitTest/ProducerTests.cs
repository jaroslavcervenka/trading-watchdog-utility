// using System.Threading;
// using Moq;
// using MT5Wrapper.Interface;
// using Watchdog.App.Interface;
// using Watchdog.App.Model;
// using Xunit;
//
// namespace Watchdog.App.UnitTest
// {
//     public class ProducerTests
//     {
//         [Fact]
//         public void QueuePerformance()
//         {
//             var tokenSource = new CancellationTokenSource();
//             
//             const string ip1 = "1.1.1.1";
//             var producer1 = CreateProducer(ip1);
//
//             const string ip2 = "2.2.2.2";
//             var producer2 = CreateProducer(ip2);
//
//             var task1 = producer1.WaitDealEventsAsync();
//         }
//         
//         private static IProducer CreateProducer(string ip)
//         {
//             var connectionConfig = new ConnectionConfig(
//                 ip, 
//                 1,
//                 "password");
//             var api = CreateMT5ApiMock();
//             var maxTimeDeltaMsc = 1000;
//             var producer = new Producer(api, connectionConfig, maxTimeDeltaMsc);
//
//             return producer;
//         }
//
//         private static IMT5Api CreateMT5ApiMock()
//         {
//             var apiMock = new Mock<IMT5Api>(MockBehavior.Strict);
//
//             return apiMock.Object;
//         }
//     }
// }