using System;
using PriceCalculator.Infrastructure;
using Xunit;
using static PriceCalculator.Infrastructure.Option;
namespace PriceCalculatorTests.Infrastructure;

public class OptionExtensionsTests
{
    [Fact]
    public void TesFoldWithSome()
    {
        var a = Some(2);
        Assert.Equal(9, a.Fold(7, (seed, value) => seed + value));
    }
    [Fact]
    public void TesFoldWithNone()
    {
        var a = None<int>();
        Assert.Equal(7, a.Fold(7, (seed, value) => seed + value));
    }
}
