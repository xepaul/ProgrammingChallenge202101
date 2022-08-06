using System;
using System.Collections.Generic;

namespace PriceCalculator.Infrastructure;

public struct Maybe<T> : IEquatable<Maybe<T>>
{
    private readonly T _justValue;

    private Maybe(T value)
    {
        _justValue = value;
        IsJust = true;
    }

    public bool IsJust { get; }
    public bool IsNothing => !IsJust;

    public static Maybe<T> Nothing { get; } = default;
    public static Maybe<T> Just(T value) => new Maybe<T>(value);

    public TR maybe<TR>(Func<T, TR> justFunc, Func<TR> nothingFunc)
        => IsJust ? justFunc(_justValue) : nothingFunc();

    public bool Equals(Maybe<T> other) =>
        IsJust == other.IsJust && EqualityComparer<T>.Default.Equals(_justValue, other._justValue);

    public override bool Equals(object? other)
        => other is Maybe<T> m && Equals(m);

    public override int GetHashCode() =>
        IsJust ? EqualityComparer<T>.Default.GetHashCode(_justValue) : 0;

    public static bool operator ==(Maybe<T> first, Maybe<T> second) =>
        first.Equals(second);

    public static bool operator !=(Maybe<T> first, Maybe<T> second) =>
        !first.Equals(second);

    public override string ToString() => IsJust ? $"Just({_justValue})" : "Nothing";
}

public static class Maybe
{
    public static Maybe<T> Just<T>(T value) => Maybe<T>.Just(value);
    public static Maybe<T> Nothing<T>() => Maybe<T>.Nothing;

    public static bool IsNothing<T>(Maybe<T> value) => !value.IsJust;

    public static Maybe<TA> Pure<TA>(TA value) => Maybe<TA>.Just(value);

    public static bool IsJust<TA>(Maybe<TA> o) => o.IsJust;

    public static Maybe<T> Cond<T>(bool cond, T value) => cond ? Maybe<T>.Just(value) : Maybe<T>.Nothing;
}
