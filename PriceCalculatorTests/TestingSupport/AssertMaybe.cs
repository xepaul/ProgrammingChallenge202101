using System;
using System.Collections.Generic;
using System.Linq;
using PriceCalculator.Infrastructure;
using Xunit;

namespace PriceCalculatorTests.TestingSupport;

public static class AssertMaybe
{
    public static void SequenceEqual<T>(Maybe<IEnumerable<T>> expected, Maybe<IEnumerable<T>> result)
    {
        Action f =
        (expected.IsJust, result.IsJust) switch
        {
            (true, true) => () => result.Iter2(expected,
                (e, r) =>
                    Assert.Collection(e, r.Select(v => new Action<T>(e => Assert.Equal(e, v))).ToArray())),
            (true, false) => () => Assert.True(false, "Expected Just"),
            (false, true) => () => Assert.True(false, "Expected Nothing"),
            (false, false) => () => Assert.True(true),
        };
        f();
    }
}
