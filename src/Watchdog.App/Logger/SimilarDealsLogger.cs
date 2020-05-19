using System.Collections.Generic;
using System.Text;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Watchdog.App.Abstractions;

namespace Watchdog.App.Logger
{
    public class SimilarDealsLogger<T> : ISimilarDealsLogger<T>
    {
        private readonly ILogger<SimilarDealsLogger<T>> _logger;
        
        public SimilarDealsLogger(ILogger<SimilarDealsLogger<T>> logger)
        {
            Guard.Against.Null(logger, nameof(logger));
            
            _logger = logger;
        }
        
        public void Log(T deal, IEnumerable<T> similarDeals)
        {
            Guard.Against.Null(deal, nameof(deal));
            Guard.Against.Null(similarDeals, nameof(similarDeals));
            
            var sb = new StringBuilder();
            sb.AppendLine($"Found similar deals ({deal.ToString()}):");
            foreach (var similarDeal in similarDeals)
            {
                sb.AppendLine(similarDeal?.ToString());
            }
            _logger.LogWarning(sb.ToString());
        }
    }
}