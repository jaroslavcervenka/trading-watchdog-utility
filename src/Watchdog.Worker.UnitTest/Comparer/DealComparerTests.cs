using System;
using FluentAssertions;
using Watchdog.App.Abstractions;
using Watchdog.App.Comparer;
using Watchdog.App.Model;
using Xunit;

namespace Watchdog.App.UnitTest.Comparer
{
    public class DealComparerTests
    {
        private const ulong Login = 1; 
        private const int MaxOpenTimeDelta = 1;
        private const decimal MaxRatio = 5;
        private readonly AppOptions _appOptions = new AppOptions(
            new [] {"server"}, 1, "pass", MaxRatio, MaxOpenTimeDelta);
        
        [Fact]
        public void Compare_1_IsSimilar()
        {
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            const bool sellActionA = true;
            const bool sellActionB = sellActionA;
            var  volumeA = Convert.ToUInt64(0.2 / 0.0001);
            var  volumeB = Convert.ToUInt64(0.21 / 0.0001);
            const decimal balanceA = 10000;
            const decimal balanceB = 10000;
            const string symbolA = "GBPUSD";
            const string symbolB = symbolA;
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA + 1;
            var dealA = new Deal(Login, sellActionA, volumeA, balanceA, symbolA, timeMscA);
            var dealB = new Deal(Login, sellActionB, volumeB, balanceB, symbolB, timeMscB);
            
            //Act
            var result = comparer.Compare(dealA, dealB);

            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void Compare_2_IsSimilar()
        {
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            const bool sellActionA = true;
            const bool sellActionB = sellActionA;
            var  volumeA = Convert.ToUInt64(0.2 / 0.0001);
            var  volumeB = Convert.ToUInt64(0.4 / 0.0001);
            const decimal balanceA = 10000;
            const decimal balanceB = 20000;
            const string symbolA = "GBPUSD";
            const string symbolB = symbolA;
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA + 1;
            var dealA = new Deal(Login, sellActionA, volumeA, balanceA, symbolA, timeMscA);
            var dealB = new Deal(Login, sellActionB, volumeB, balanceB, symbolB, timeMscB);
            
            //Act
            var result = comparer.Compare(dealA, dealB);

            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void Compare_3_IsSimilar()
        {
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            const bool sellActionA = true;
            const bool sellActionB = sellActionA;
            var  volumeA = Convert.ToUInt64(0.21 / 0.0001);
            var  volumeB = Convert.ToUInt64(0.4 / 0.0001);
            const decimal balanceA = 10000;
            const decimal balanceB = 20000;
            const string symbolA = "GBPUSD";
            const string symbolB = symbolA;
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA;
            var dealA = new Deal(Login, sellActionA, volumeA, balanceA, symbolA, timeMscA);
            var dealB = new Deal(Login, sellActionB, volumeB, balanceB, symbolB, timeMscB);
            
            //Act
            var result = comparer.Compare(dealA, dealB);

            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void Compare_NoSimilar()
        {
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            const bool sellActionA = false;
            const bool sellActionB = true;
            var  volumeA = Convert.ToUInt64(1 / 0.0001);
            var  volumeB = Convert.ToUInt64(0.21 / 0.0001);
            const decimal balanceA = 10000;
            const decimal balanceB = 10000;
            const string symbolA = "EURUSD";
            const string symbolB = "GBPUSD";
            const long timeMscA = 1575556990;
            const long timeMscB = 1575556992;
            var dealA = new Deal(Login, sellActionA, volumeA, balanceA, symbolA, timeMscA);
            var dealB = new Deal(Login, sellActionB, volumeB, balanceB, symbolB, timeMscB);
            
            //Act
            var result = comparer.Compare(dealA, dealB);

            //Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void Compare_DealsAreEqual_NoSimilar()
        {
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            const bool sellActionA = true;
            const bool sellActionB = sellActionA;
            var  volumeA = Convert.ToUInt64(0.21 / 0.0001);
            var  volumeB = volumeA;
            const decimal balanceA = 10000;
            const decimal balanceB = balanceA;
            const string symbolA = "GBPUSD";
            const string symbolB = symbolA;
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA;
            var dealA = new Deal(Login, sellActionA, volumeA, balanceA, symbolA, timeMscA);
            var dealB = new Deal(Login, sellActionB, volumeB, balanceB, symbolB, timeMscB);
            
            //Act
            var result = comparer.Compare(dealA, dealB);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsOlderThanLimit_True()
        {
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA - 5;
            
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            var deallatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscA);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscB);
            
            //Act
            var result = comparer.IsOlderThanLimit(deallatest, dealOlder);

            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void IsOlderThanLimit_False()
        {
            const long timeMscA = 1575556992;
            const long timeMscB = timeMscA - 1;
            
            //Arrange
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            var dealLatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscA);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscB);
            
            //Act
            var result = comparer.IsOlderThanLimit(dealLatest, dealOlder);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanBeRemoved_DealIsOlderThan2Sec_True()
        {
            //Arrange
            const long timeMscLatest = 1575556992;
            const long timeMscOlderThan2Sec = timeMscLatest - 3;
            var dealLatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscLatest);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscOlderThan2Sec);
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            
            //Act
            var result = comparer.CanBeRemoved(dealLatest, dealOlder);

            //Assert
            result.Should().BeTrue();
        }
        
        [Fact]
        public void CanBeRemoved_DealIsOlder2Sec_False()
        {
            //Arrange
            const long timeMscLatest = 1575556992;
            const long timeMscOlderThan2Sec = timeMscLatest - 2;
            var dealLatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscLatest);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscOlderThan2Sec);
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            
            //Act
            var result = comparer.CanBeRemoved(dealLatest, dealOlder);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void CanBeRemoved_DealIsOlder1Sec_False()
        {
            //Arrange
            const long timeMscLatest = 1575556992;
            const long timeMscOlderThan2Sec = timeMscLatest - 1;
            var dealLatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscLatest);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscOlderThan2Sec);
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);

            //Act
            var result = comparer.CanBeRemoved(dealLatest, dealOlder);

            //Assert
            result.Should().BeFalse();
        }
        
        [Fact]
        public void CanBeRemoved_DealIsSameTime_False()
        {
            //Arrange
            const long timeMscLatest = 1575556992;
            const long timeMscOlderThan2Sec = timeMscLatest;
            var dealLatest = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscLatest);
            var dealOlder = new Deal(Login, true, 1000, 10000, "EURUSD", timeMscOlderThan2Sec);
            IDealComparer<Deal> comparer = new DealComparer(_appOptions);
            
            //Act
            var result = comparer.CanBeRemoved(dealLatest, dealOlder);

            //Assert
            result.Should().BeFalse();
        }
    }
}