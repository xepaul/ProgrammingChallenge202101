using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace PriceCalculator.Infrastructure;

public static class ImmutableDictionaryExtensions
{
    /// <summary>
    /// <para> Try get the value for the key in the dictionary - return Some value if found or None </para>
    /// <para> improvement over using out parameters and null values  </para>
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static Option<TValue> TryGetValue<TKey, TValue>(this ImmutableDictionary<TKey, TValue> dictionary, TKey key) =>
        Option.Cond(dictionary.TryGetValue(key, out var value), value);
}
