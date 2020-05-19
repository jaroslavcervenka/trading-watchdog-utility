namespace Watchdog.App.Abstractions
{
    public interface IInstanceIdGenerator<T>
    {
        int GetNewId();
    }
}