using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PriceCalculator.Infrastructure;

public static class MaybeExtensions
{
    /// <summary>
    /// Performs a Select/Map on the value inside the maybe
    /// </summary>
    /// <param name="m">The input maybe.</param>
    /// <param name="mapper">A function to apply to the maybe value.</param>
    /// <typeparam name="T">the generic type of the input maybe</typeparam>
    /// <typeparam name="TR">the generic type of the returned maybe</typeparam>
    /// <returns>An maybe of the input value after applying the mapping function, or Nothing if the input is Nothing.</returns>
    public static Maybe<TR> Map<T, TR>(this Maybe<T> m, Func<T, TR> mapper) =>
        m.maybe(value => Maybe<TR>.Just(mapper(value)), () => Maybe<TR>.Nothing);

    /// <summary>
    /// Performs a SelectMany/Bind on the value inside the maybe
    /// </summary>
    /// <param name="m"></param>
    /// <param name="mapper"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <returns></returns>
    public static Maybe<TR> Bind<T, TR>(this Maybe<T> m, Func<T, Maybe<TR>> mapper) =>
        m.maybe(mapper,
            () => Maybe<TR>.Nothing);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="m"></param>
    /// <param name="seed">The initial state.</param>
    /// <param name="mapper">A function to update the state data when given a value from an maybe.</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <returns>The original state if the maybe is Nothing, otherwise it returns the updated state with the folder and the maybe value.</returns>
    public static TR Fold<T, TR>(this Maybe<T> m, TR seed, Func<TR, T, TR> mapper) =>
        m.maybe(x => mapper(seed, x), () => seed);

    /// <summary>
    /// perform a side effect if the maybe is a Nothing
    /// </summary>
    /// <param name="maybe">The input maybe.</param>
    /// <param name="doAction"> the side effect Action</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>the given maybe</returns>
    public static Maybe<T> DoNothing<T>(this Maybe<T> maybe, Action doAction) =>
        maybe.maybe(x => maybe,
                      () =>
                      {
                          doAction();
                          return maybe;
                      });
    public static Maybe<T> DoJust<T>(this Maybe<T> maybe, Action<T> doAction) =>
        maybe.maybe(value =>
            {
                doAction(value);
                return maybe;
            },
            () => maybe);
    /// <summary>
    /// Gets the value of the maybe if the maybe is Just, otherwise returns the specified default value.
    /// </summary>
    /// <param name="maybe">The input maybe.</param>
    /// <param name="value">The specified default value.</param>
    /// <typeparam name="T">type of the maybe element</typeparam>
    /// <returns>The maybe if the maybe is Just, else the default value.</returns>
    public static T DefaultValue<T>(this Maybe<T> maybe, T value) => maybe.maybe(x => x, () => value);

    /// <summary>
    /// Gets the value of the maybe if the maybe is Just, otherwise evaluates defThunk and returns the result.
    /// </summary>
    /// <param name="maybe">The input maybe.</param>
    /// <param name="defaultThunk">A thunk that provides a default value when evaluated.</param>
    /// <typeparam name="T">type of the maybe element</typeparam>
    /// <returns>The maybe if the maybe is Just, else the result of evaluating defThunk.</returns>
    public static T DefaultWith<T>(this Maybe<T> maybe, Func<T> defaultThunk) =>
        maybe.maybe(x => x, defaultThunk);

    public static void Iter<T>(this Maybe<T> maybe, Action<T> sideEffect) =>
        maybe.maybe(value =>
        {
            sideEffect(value);
            return 1;
        }, () => 1);

    /// <summary>
    /// Filter's maybe
    /// </summary>
    /// <param name="maybe"></param>
    /// <param name="f"></param>
    /// <typeparam name="a"></typeparam>
    /// <returns>The input if the predicate evaluates to true; otherwise, Nothing.</returns>
    public static Maybe<a> Filter<a>(this Maybe<a> maybe, Func<a, bool> f) => maybe.Bind(v => f(v) ? maybe : Maybe<a>.Nothing);
    public static Maybe<c> ZipWith<a, b, c>(this Maybe<a> maybeA, Maybe<b> maybeB, Func<a, b, c> f) => maybeA.Map2(maybeB, f);

    public static void Iter2<a, b>(this Maybe<a> maybeA, Maybe<b> maybeB, Action<a, b> f) =>
        maybeA.Map2(maybeB, (a, b) =>
         {
            f(a, b);
            return 0;
        });
    public static Maybe<c> Map2<a, b, c>(this Maybe<a> maybeA, Maybe<b> maybeB, Func<a, b, c> f) =>
        maybeA.maybe(x => maybeB.maybe(y => Maybe<c>.Just(f(x, y)), () => Maybe<c>.Nothing),
            () => Maybe<c>.Nothing);
    public static bool Exists<a>(this Maybe<a> o, Func<a, bool> f) => o.maybe(f, () => false);
    public static Either<a, b> NoteWith<a, b>(this Maybe<b> m, Func<a> left) => m.maybe(Either<a>.Pure, () => Either<b>.Left(left()));

    /// <summary>
    /// Tag the Nothing value of a Maybe - transform to an Either
    /// </summary>
    /// <param name="m"></param>
    /// <param name="left"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="b"></typeparam>
    /// <returns></returns>
    public static Either<a, b> Note<a, b>(this Maybe<b> m, a left) => m.maybe(Either<a>.Pure, () => Either<b>.Left(left));
}
