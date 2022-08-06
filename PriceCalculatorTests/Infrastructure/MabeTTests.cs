using PriceCalculator.Infrastructure;
using Xunit;
namespace PriceCalculatorTests.Infrastructure;

public class MaybeTTests
{
    [Fact]
    public void TestMaybeEquality()
    {
        var a = Maybe.Just(2);
        var b = Maybe.Just(2);
        Assert.True(a == b);
    }

    [Fact]
    public void TestMaybeJustIsJustTrue() => Assert.True(Maybe.Just(2).IsJust);
    public void TestMaybeJustIsNothingFalse() => Assert.False(Maybe.Just(2).IsNothing);
    [Fact]
    public void TestMaybeNothingIsNothingTrue() => Assert.True(Maybe<int>.Nothing.IsNothing);
    [Fact]
    public void TestMaybeNothingIsJustFalse() => Assert.False(Maybe<int>.Nothing.IsJust);
}
