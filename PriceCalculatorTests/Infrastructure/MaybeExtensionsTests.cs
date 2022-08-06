using System;
using PriceCalculator.Infrastructure;
using Xunit;
using static PriceCalculator.Infrastructure.Maybe;
namespace PriceCalculatorTests.Infrastructure;

public class MaybeExtensionsTests
{
    [Fact]
    public void TesFoldWithJust()
    {
        var a = Just(2);
        Assert.Equal(9, a.Fold(7, (seed, value) => seed + value));
    }
    [Fact]
    public void TesFoldWithNothing()
    {
        var a = Nothing<int>();
        Assert.Equal(7, a.Fold(7, (seed, value) => seed + value));
    }
}
