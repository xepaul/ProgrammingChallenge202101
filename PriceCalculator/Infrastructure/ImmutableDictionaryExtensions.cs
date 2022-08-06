using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PriceCalculator.Infrastructure;

public static class ImmutableDictionaryExtensions
{
    /// <summary>
    /// <para> Try get the value for the key in the dictionary - return Jsut value if found or Nothing </para>
    /// <para> improvement over using out parameters and null values  </para>
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Maybe<TValue> TryGetValue<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dictionary, TKey key) =>
        Maybe.Cond(dictionary.TryGetValue(key, out var value), value);
}
