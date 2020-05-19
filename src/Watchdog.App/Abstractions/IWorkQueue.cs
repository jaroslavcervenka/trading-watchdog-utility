namespace Watchdog.App.Abstractions
{
    public interface IWorkQueue<T>
    {
        public int Count { get; }
        public void Enqueue (T item);

        public bool TryDequeue (out T result);

        public System.Collections.Generic.IEnumerator<T> GetEnumerator ();

        public void Cleanup(T latestDeal);
    }
}