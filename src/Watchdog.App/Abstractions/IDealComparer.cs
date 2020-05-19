namespace Watchdog.App.Abstractions
{
    public interface IDealComparer<in T> where T : struct
    {
        bool Compare(T dealA, T dealB);

        bool IsOlderThanLimit(T latestDeal, T previousDeal);

        bool CanBeRemoved(T latestDeal, T previousDeal);
    }
}