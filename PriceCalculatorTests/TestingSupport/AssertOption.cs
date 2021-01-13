using System;
using System.Collections.Generic;
using System.Linq;
using PriceCalculator.Infrastructure;
using Xunit;

namespace PriceCalculatorTests.TestingSupport
{
    public static class AssertOption
    {
        public static void SequenceEqual<T>(Option<IEnumerable<T>> expected, Option<IEnumerable<T>> result)
        {
            Action f =
            (expected.IsSome, result.IsSome) switch
            {
                (true, true) => ()=> result.Iter2(expected,
                    (e, r) => 
                        Assert.Collection(e, r.Select(v => new Action<T>(e => Assert.Equal(e, v))).ToArray())),
                (true, false) => ()=>Assert.True(false, "Expected Some"),
                (false, true) => ()=> Assert.True(false, "Expected None"),
                (false, false) => ()=>Assert.True(true),
            };
          f();
        }
    }
}