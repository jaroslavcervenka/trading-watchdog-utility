using System.Collections.Generic;

namespace Watchdog.App.Abstractions
{
    public interface ISimilarDealsLogger<in T>
    {
        void Log(T deal, IEnumerable<T> similarDeals);
    }
}