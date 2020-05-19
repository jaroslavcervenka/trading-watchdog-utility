using System;

namespace Watchdog.App.Model
{
    public readonly struct Deal : IEquatable<Deal>
    {
        public ulong Login { get; }
        
        public bool SellAction { get; }

        public decimal VolumeToBalanceRatio { get; }
        
        public string Symbol { get; }

        public long TimeMsc { get; }
        

        public Deal(
            ulong login,
            bool sellAction,
            ulong volume,
            decimal balance,
            string symbol,
            long timeMsc
            )
        {
            Login = login;
            SellAction = sellAction;
            VolumeToBalanceRatio = ComputeVolumeToBalanceRatio(volume, balance);
            Symbol = symbol;
            TimeMsc  = timeMsc;
        }

        private static decimal ComputeVolumeToBalanceRatio(ulong volume, decimal balance)
        {
            var lots = Convert.ToDecimal(volume * 0.0001);
            return (lots / balance) * 100;
        }

        public override string ToString()
        {
            return $"{Login}, {SellAction}, {VolumeToBalanceRatio}, {Symbol}, {TimeMsc}";
        }

        public bool Equals(Deal other)
        {
            return Login == other.Login 
                   && SellAction == other.SellAction 
                   && VolumeToBalanceRatio == other.VolumeToBalanceRatio 
                   && Symbol == other.Symbol 
                   && TimeMsc == other.TimeMsc;
        }

        public override bool Equals(object? obj)
        {
            return obj is Deal other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Login.GetHashCode();
                hashCode = (hashCode * 397) ^ SellAction.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumeToBalanceRatio.GetHashCode();
                hashCode = (hashCode * 397) ^ Symbol.GetHashCode();
                hashCode = (hashCode * 397) ^ TimeMsc.GetHashCode();
                return hashCode;
            }
        }
    }
}