//
// using System;
// using System.Threading;
// using System.Threading.Tasks;
// using FluentResults;
// using Microsoft.Extensions.Logging;
// using ParkSquare.Testing.Generators;
// using Watchdog.App.Abstractions;
// using Watchdog.App.TradingApi;
// using Watchdog.App.Model;
// using Watchdog.App.Producer;
// using Xunit;
// using Xunit.Abstractions;
//
// namespace Watchdog.App.UnitTest
// {
//     public class AsyncProducerTests
//     {
//         private readonly ITestOutputHelper _outputHelper;
//
//         public AsyncProducerTests(ITestOutputHelper output)
//         {
//             _outputHelper = output;
//         }
//         
//         [Fact]
//         public async Task MultiProducers()
//         {
//             const int dealsCount = 1_000_000;
//             var expectedDealsCount = 2 * dealsCount;
//             var tokenSource = new CancellationTokenSource();
//             
//             var producer1 = CreateProducer("1.1.1.1", dealsCount);
//             var producer2 = CreateProducer("2.2.2.2", dealsCount);
//             var consumer = CreateConsumer(expectedDealsCount, tokenSource);
//         
//             producer1.Subscribe(consumer.Consume, tokenSource.Token);
//             producer2.Subscribe(consumer.Consume, tokenSource.Token);
//
//             await consumer.WaitForFinish(tokenSource.Token);
//             
//             _outputHelper.WriteLine($"Consumer counter: {consumer.Counter}");
//         }
//
//         private DealProducer CreateProducer(string ip, int dealsCount)
//         {
//             var connectionConfig = new ConnectionConfig(
//                  ip, 
//                  1,
//                  "password");
//             var api = CreateApi(dealsCount);
//             var jobQueue = CreateQueue();
//             var logger = new XunitLogger<DealProducer>(_outputHelper);
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
//         private DealConsumer CreateConsumer(
//             int expectedDealsCount,
//             CancellationTokenSource tokenSource)
//         {
//             var logger = new XunitLogger<DealConsumer>(_outputHelper);
//             var consumer = new DealConsumer(logger, expectedDealsCount, tokenSource);
//
//             return consumer;
//         }
//
//         private IJobQueue<Deal> CreateQueue()
//         {
//             var logger = new XunitLogger<AsyncProducerConsumerQueue<Deal>>(_outputHelper);
//             var queue = new AsyncProducerConsumerQueue<Deal>(
//                 logger);
//
//             return queue;
//         }
//         
//         private IApi CreateApi(int dealsCount)
//         {
//             var logger = new XunitLogger<TestApi>(_outputHelper);
//             return new TestApi(dealsCount, logger);
//         }
//
//         private class TestApi : IApi
//         {
//             public event EventHandler<DealAddEventArgs> DealAdded = delegate {  };
//
//             private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
//             private readonly ILogger<TestApi> _logger;
//             private readonly int _dealsCount;
//
//             public TestApi(int dealsCount, ILogger<TestApi> logger)
//             {
//                 _dealsCount = dealsCount;
//                 _logger = logger;
//             }
//             
//             public void Connect(ConnectionConfig connectionConfig)
//             {
//                 var token = _cancellationTokenSource.Token;
//
//                 Task.Run(() =>
//                 {
//                     for (int i = 0; i < _dealsCount; i++)
//                     {
//                         if (token.IsCancellationRequested)
//                         {
//                             //token.ThrowIfCancellationRequested();
//                             _logger.LogInformation($"Cancelled deals: {i}");
//                             return;
//                         }
//                         var deal = new Deal(
//                             (ulong)LongGenerator.AnyPositiveLong(),
//                             BooleanGenerator.IsCoinTossHeads(),
//                             (ulong)IntegerGenerator.AnyPositiveInteger(),
//                             DecimalGenerator.AnyPositiveCurrencyAmount(),
//                             StringGenerator.SequenceOfAlphas(6),
//                             LongGenerator.AnyPositiveLong());
//                         
//                         DealAdded.Invoke(this, new DealAddEventArgs(deal));
//
//                        //Thread.Sleep(1);
//                     }
//                     
//                     _logger.LogInformation($"Finished deals: {_dealsCount}");
//                 }, token);
//             }
//
//             public void Disconnect()
//             {
//                 _cancellationTokenSource.Cancel();
//             }
//
//             public Result<decimal> GetUserBalance(ulong login)
//             {
//                 return Results.Ok((decimal)1000);
//             }
//         }
//
//         private class XunitLogger<T> : ILogger<T>, IDisposable
//         {
//             private ITestOutputHelper _output;
//
//             public XunitLogger(ITestOutputHelper output)
//             {
//                 _output = output;
//             }
//             
//             public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//             {
//                 _output.WriteLine($"{logLevel}: {state.ToString()}");
//             }
//
//             public bool IsEnabled(LogLevel logLevel)
//             {
//                 return true;
//             }
//
//             public IDisposable BeginScope<TState>(TState state)
//             {
//                 return this;
//             }
//             
//             public void Dispose()
//             {
//             }
//         }
//     }
// }