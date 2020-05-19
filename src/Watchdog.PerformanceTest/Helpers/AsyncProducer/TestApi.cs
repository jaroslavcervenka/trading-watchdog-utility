using System;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using ParkSquare.Testing.Generators;
using Watchdog.App.Abstractions;
using Watchdog.App.Api;
using Watchdog.App.Model;

namespace Watchdog.PerformanceTest.Helpers.AsyncProducer
{  
    internal class TestApi: IApi<Deal>
  {
      public event EventHandler<DealAddEventArgs<Deal>> DealAdded = delegate {  };

      private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
      private readonly int _dealsCount;
      private readonly ILogger<TestApi> _logger;

      public TestApi(ILogger<TestApi> logger, int dealsCount)
      {
          _dealsCount = dealsCount;
          _logger = logger;
      }
      
      public void Connect(ConnectionConfig connectionConfig)
      {
          var token = _cancellationTokenSource.Token;

          Task.Run(() =>
          {
              for (int i = 0; i < _dealsCount; i++)
              {
                  if (token.IsCancellationRequested)
                  {
                      token.ThrowIfCancellationRequested();
                      _logger.LogInformation($"Cancelled deals: {i}");
                  }
                  var deal = new Deal(
                      (ulong)LongGenerator.AnyPositiveLong(),
                      BooleanGenerator.IsCoinTossHeads(),
                      (ulong)IntegerGenerator.AnyPositiveInteger(),
                      DecimalGenerator.AnyPositiveCurrencyAmount(),
                      StringGenerator.SequenceOfAlphas(6),
                      LongGenerator.AnyPositiveLong());
                  
                  DealAdded.Invoke(this, new DealAddEventArgs<Deal>(deal));

                 //Thread.Sleep(1);
              }
              
              _logger.LogInformation($"Finished deals: {_dealsCount}");
          }, token);
      }

      public void Disconnect()
      {
          _cancellationTokenSource.Cancel();
      }

      public Result<decimal> GetUserBalance(ulong login)
      {
          return Results.Ok((decimal)1000);
      }
  }
}