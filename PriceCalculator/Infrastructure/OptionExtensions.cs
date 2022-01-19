using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace PriceCalculator.Infrastructure;

public static class OptionExtensions
{
    /// <summary>
    /// Performs a Select/Map on the value inside the option
    /// </summary>
    /// <param name="option">The input option.</param>
    /// <param name="mapper">A function to apply to the option value.</param>
    /// <typeparam name="T">the generic type of the input option</typeparam>
    /// <typeparam name="TR">the generic type of the returned option</typeparam>
    /// <returns>An option of the input value after applying the mapping function, or None if the input is None.</returns>
    public static Option<TR> Map<T, TR>(this Option<T> option, Func<T, TR> mapper) =>
        option.option(value => Option<TR>.Some(mapper(value)),
            () => Option<TR>.None);

    /// <summary>
    /// Performs a SelectMany/Bind on the value inside the option
    /// </summary>
    /// <param name="option"></param>
    /// <param name="mapper"></param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <returns></returns>
    public static Option<TR> Bind<T, TR>(this Option<T> option, Func<T, Option<TR>> mapper) =>
        option.option(mapper,
            () => Option<TR>.None);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="option"></param>
    /// <param name="seed">The initial state.</param>
    /// <param name="mapper">A function to update the state data when given a value from an option.</param>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TR"></typeparam>
    /// <returns>The original state if the option is None, otherwise it returns the updated state with the folder and the option value.</returns>
    public static TR Fold<T, TR>(this Option<T> option, TR seed, Func<TR, T, TR> mapper) =>
        option.option(x => mapper(seed, x), () => seed);

    /// <summary>
    /// perform a side effect if the option is a None
    /// </summary>
    /// <param name="option">The input option.</param>
    /// <param name="doAction"> the side effect Action</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>the given option</returns>
    public static Option<T> DoNone<T>(this Option<T> option, Action doAction) =>
        option.option(x => option,
                      () =>
                      {
                          doAction();
                          return option;
                      });
    public static Option<T> DoSome<T>(this Option<T> option, Action<T> doAction) =>
        option.option(value =>
            {
                doAction(value);
                return option;
            },
            () => option);
    /// <summary>
    /// Gets the value of the option if the option is Some, otherwise returns the specified default value.
    /// </summary>
    /// <param name="option">The input option.</param>
    /// <param name="value">The specified default value.</param>
    /// <typeparam name="T">type of the option element</typeparam>
    /// <returns>The option if the option is Some, else the default value.</returns>
    public static T DefaultValue<T>(this Option<T> option, T value) =>
       option.option(x => x, () => value);

    /// <summary>
    /// Gets the value of the option if the option is Some, otherwise evaluates defThunk and returns the result.
    /// </summary>
    /// <param name="option">The input option.</param>
    /// <param name="defaultThunk">A thunk that provides a default value when evaluated.</param>
    /// <typeparam name="T">type of the option element</typeparam>
    /// <returns>The option if the option is Some, else the result of evaluating defThunk.</returns>
    public static T DefaultWith<T>(this Option<T> option, Func<T> defaultThunk) =>
        option.option(x => x, defaultThunk);

    public static void Iter<T>(this Option<T> option, Action<T> sideEffect) =>
        option.option(value =>
        {
            sideEffect(value);
            return 1;
        }, () => 1);

    /// <summary>
    /// Filter's option
    /// </summary>
    /// <param name="option"></param>
    /// <param name="f"></param>
    /// <typeparam name="a"></typeparam>
    /// <returns>The input if the predicate evaluates to true; otherwise, None.</returns>
    public static Option<a> Filter<a>(this Option<a> option, Func<a, bool> f) => option.Bind(v => f(v) ? option : Option<a>.None);
    public static Option<c> ZipWith<a, b, c>(this Option<a> opta, Option<b> optb, Func<a, b, c> f) =>
        opta.Map2(optb, f);

    public static void Iter2<a, b>(this Option<a> opta, Option<b> optb, Action<a, b> f) =>
        opta.Map2(optb, (a, b) =>
         {
            f(a, b);
            return 0;
        });
    public static Option<c> Map2<a, b, c>(this Option<a> opta, Option<b> optb, Func<a, b, c> f) =>
        opta.option(x => optb.option(y => Option<c>.Some(f(x, y)), () => Option<c>.None),
            () => Option<c>.None);
    public static bool Exists<a>(this Option<a> o, Func<a, bool> f) => o.option(f, () => false);
    public static Either<a, b> NoteWith<a, b>(this Option<b> o, Func<a> left) => o.option(Either<a>.Pure, () => Either<b>.Left(left()));

    /// <summary>
    /// Tag the None value of a Option - transform to an Either
    /// </summary>
    /// <param name="o"></param>
    /// <param name="left"></param>
    /// <typeparam name="a"></typeparam>
    /// <typeparam name="b"></typeparam>
    /// <returns></returns>
    public static Either<a, b> Note<a, b>(this Option<b> o, a left) => o.option(Either<a>.Pure, () => Either<b>.Left(left));
}
