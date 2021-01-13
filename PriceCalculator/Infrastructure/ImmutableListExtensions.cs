using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PriceCalculator.Infrastructure
{
    public static class ImmutableListExtensions
    {
        public static ImmutableList<T> MapAt<T>(this ImmutableList<T> immutableList, int index, Func<T, T> mapper) =>
            immutableList.Select((item, iIndex) => index == iIndex ? mapper(item) : item).ToImmutableList();
    }
}