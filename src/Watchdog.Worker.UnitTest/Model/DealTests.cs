using FluentAssertions;
using Watchdog.App.Model;
using Xunit;

namespace Watchdog.App.UnitTest.Model
{
    public class DealTests
    {
        [Fact]
        public void Constructor_SetProperties()
        {
            //Arrange
            const ulong expectedLogin = 1;
            const bool expectedSellAction = true;
            const decimal expectedVolumeToBalanceRatio = (decimal) 0.0021;
            const string expectedSymbol = "EURUSD";
            const long expectedTimeMsc = 1575556992;
            const ulong volume = 2100;
            const decimal balance = 10000;

            //Act
            var result = new Deal(
                expectedLogin,
                expectedSellAction,
                volume,
                balance,
                expectedSymbol,
                expectedTimeMsc);

            //Assert
            result.Login.Should().Be(expectedLogin);
            result.SellAction.Should().Be(expectedSellAction);
            result.VolumeToBalanceRatio.Should().Be(expectedVolumeToBalanceRatio);
            result.Symbol.Should().Be(expectedSymbol);
            result.TimeMsc.Should().Be(expectedTimeMsc);
        }
    }
}