using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using static PriceCalculator.Infrastructure.Option;
namespace PriceCalculator.Infrastructure;

public static class EnumerableExtensions
{
    public static Option<int> TryFindIndex<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        using var enumerable = source.GetEnumerator();
        var result = Option<int>.None;
        var index = 0;
        while (Option.IsNone(result) && enumerable.MoveNext())
        {
            var current = enumerable.Current;
            if (predicate(current))
                result = Option<int>.Some(index);
            index++;
        }

        return result;
    }

    /// <summary>
    /// <para>Applies the given function to each element of the list. Return the list comprised of the results "x" for each element where the function returns Some(x). </para>
    /// <para> Fused Select/Map and Where/Filter</para>
    /// </summary>
    /// <param name="source">The input sequence of type T.</param>
    /// <param name="mapper">A function to transform items of type T into options of type U.</param>
    /// <typeparam name="TResult">result element type</typeparam>
    /// <typeparam name="T">source element type</typeparam>
    /// <returns>The result sequence.</returns>
    public static IEnumerable<TResult> MapOption<T, TResult>(this IEnumerable<T> source, Func<T, Option<TResult>> mapper)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var result = mapper(enumerator.Current);
            if (result.IsSome)
                yield return result.option(value => value, () => throw new Exception("can't happen!"));
        }
    }
    /// <summary>
    /// <para> Returns the first element for which the given function returns True. Return None if no such element exists. </para>
    /// <para> improves type safety of standard linq FirstOrDefault</para>
    /// </summary>
    /// <param name="source1">The input sequence.</param>
    /// <param name="predicate">A function that evaluates to a Boolean when given an item in the sequence.</param>
    /// <typeparam name="T">sequence element type</typeparam>
    /// <returns>The found element or None.</returns>
    public static Option<T> TryFind<T>(this IEnumerable<T> source1, Func<T, bool> predicate)
    {
        using var enumerator = source1.GetEnumerator();
        var result = Option<T>.None;
        while (Option.IsNone(result) && enumerator.MoveNext())
        {
            var value = enumerator.Current;
            if (predicate(value))
                result = Some(value);
        }
        return result;
    }
    /// <summary>
    /// <para>Alias of Aggregate with seed value</para>  
    /// <para>Applies an accumulator function over a sequence. The specified seed value is used as the initial accumulator value, and the specified function is used to select the result value.</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="f"></param>
    /// <param name="seed"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="r"></typeparam>
    /// <returns></returns>
    public static r Fold<a, r>(this IEnumerable<a> source, Func<r, a, r> f, r seed) => source.Aggregate<a, r>(seed, f);

    /// <summary>
    /// Extracts from a list of Either all the Left elements. All the Left elements are extracted in order.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="b"></typeparam>
    /// <returns></returns>
    public static ImmutableList<a> Lefts<a, b>(this IEnumerable<Either<a, b>> source) =>
        source.Fold((acc, v) => v.either(acc.Add, _ => acc), ImmutableList<a>.Empty);

    /// <summary>
    /// Extracts from a list of Either all the Right elements. All the Right elements are extracted in order.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="b"></typeparam>
    /// <returns></returns>
    public static ImmutableList<b> Rights<a, b>(this IEnumerable<Either<a, b>> source) =>
        source.Fold((acc, v) => v.either(_ => acc, acc.Add), ImmutableList<b>.Empty);

    /// <summary>
    /// Partitions a list of Either into two lists. All the Left elements are extracted, in order, to the first component of the output. Similarly the Right elements are extracted to the second component of the output.
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="b"></typeparam>
    /// <returns></returns>
    public static (ImmutableList<a>, ImmutableList<b>) PartitionEithers<a, b>(this IEnumerable<Either<a, b>> source) =>
        source.Fold((acc, v) =>
        {
            var (state, item) = acc;
            return v.either(x => (state.Add(x), item), x => (state, item.Add(x)));
        },
            (ImmutableList<a>.Empty, ImmutableList<b>.Empty));
}
