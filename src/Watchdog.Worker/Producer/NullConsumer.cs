using JetBrains.Annotations;

namespace Watchdog.Worker.Producer
{
    [UsedImplicitly]
    public class NullConsumer<T>
    {

        public static void Consume(T value)
        {
            
        }
    }
}