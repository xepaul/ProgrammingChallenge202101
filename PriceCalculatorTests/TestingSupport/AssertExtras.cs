using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PriceCalculatorTests.TestingSupport
{
    public static class AssertExtras
    {
        public static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> result) =>
            Assert.Collection(expected, result.Select(v => new Action<T>(e => Assert.Equal(e, v))).ToArray());
    }
}