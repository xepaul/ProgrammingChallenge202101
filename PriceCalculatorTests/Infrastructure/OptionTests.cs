using PriceCalculator.Infrastructure;
using Xunit;
using static PriceCalculator.Infrastructure.Option;
namespace PriceCalculatorTests.Infrastructure;

public class OptionTests
{
    [Fact]
    public void TestOptionEquality()
    {
        var a = Some(2);
        var b = Some(2);
        Assert.True(a == b);
    }

    [Fact]
    public void TestOptionIsSomeTrue() => Assert.True(IsSome(Some(2)));
    [Fact]
    public void TestOptionDataTypeIsSomeTrue() => Assert.True(IsSome(Some(2)));
    [Fact]
    public void TestOptionIsSomeFalse() => Assert.True(IsNone(Option<int>.None));
}
