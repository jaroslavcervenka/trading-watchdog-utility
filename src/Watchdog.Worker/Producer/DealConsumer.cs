using System.Threading;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.Worker.Model;

namespace Watchdog.Worker.Producer
{
    public class DealConsumer
    {
        private readonly ILogger<DealConsumer> _logger;

        public int Counter;
        
        public DealConsumer(
            ILogger<DealConsumer> logger
            )
        {
            Guard.Against.Null(logger, nameof(logger));
            
            _logger = logger;
        }

        public void Consume(Deal deal)
        {
            var message = $"DEAL {deal.Login}, {deal.Symbol}, {deal.Volume}, {deal.SellAction}, {deal.TimeMsc}";

            Interlocked.Increment(ref Counter);
            //_logger.LogInformation(message);
        }
    }
}