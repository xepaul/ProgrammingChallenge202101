using PriceCalculator.Infrastructure;
using Xunit;
namespace PriceCalculatorTests.Infrastructure
{
    public class OptionTTests
    {
        [Fact]
        public void TestOptionEquality()
        {
            var a = Option.Some(2);
            var b = Option.Some(2);
            Assert.True(a==b);
        }

        [Fact]
        public void TestOptionIsSomeTrue() => Assert.True(Option.Some(2).IsSome);
        [Fact]
        public void TestOptionIsSomeFalse() => Assert.True(Option<int>.None.IsNone);
    }
}