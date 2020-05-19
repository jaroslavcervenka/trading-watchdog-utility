using System.Threading.Tasks;

namespace Watchdog.App.Api
{
    public delegate Task AsyncDealEventHandler<T>(object sender, DealAddEventArgs<T> args);
}