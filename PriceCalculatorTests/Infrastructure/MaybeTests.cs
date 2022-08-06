using PriceCalculator.Infrastructure;
using Xunit;
using static PriceCalculator.Infrastructure.Maybe;
namespace PriceCalculatorTests.Infrastructure;

public class MaybeTests
{
    [Fact]
    public void TestMaybeEquality()
    {
        var a = Just(2);
        var b = Just(2);
        Assert.True(a == b);
    }

    [Fact]
    public void TestMaybeIsJustTrue() => Assert.True(IsJust(Just(2)));
    [Fact]
    public void TestMaybeJustDataTypeIsJustTrue() => Assert.True(IsJust(Just(2)));
    [Fact]
    public void TestJustIsSomeFalse() => Assert.True(IsNothing(Maybe<int>.Nothing));
}
