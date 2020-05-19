using System;

namespace Watchdog.Worker.Model
{
    public readonly struct Deal
    {
        public ulong Login { get; }
        
        public bool SellAction { get; }

        public ulong Volume { get; }
        
        public string Symbol { get; }

        public long TimeMsc { get; }
        

        public Deal(
            ulong login,
            bool sellAction,
            ulong volume,
            string symbol,
            long timeMsc
            )
        {
            Login = login;
            SellAction = sellAction;
            Volume = volume;
            Symbol = symbol;
            TimeMsc  = timeMsc;
        }
    }
}