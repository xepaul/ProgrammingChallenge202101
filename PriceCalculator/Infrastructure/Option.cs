using System;
using System.Collections.Generic;

namespace PriceCalculator.Infrastructure
{
    public struct Option<T> : IEquatable<Option<T>>
    {
        private readonly T _someValue;

        private Option(T value)
        {
            _someValue = value;
            IsSome = true;
        }

        public bool IsSome { get; }
        public bool IsNone => !IsSome;

        public static Option<T> None { get; } = default;
        public static Option<T> Some(T value) => new Option<T>(value);

        public TR option<TR>(Func<T, TR> someFunc, Func<TR> noneFunc)
            => IsSome ? someFunc(_someValue) : noneFunc();

        public bool Equals(Option<T> other) =>
            IsSome == other.IsSome && EqualityComparer<T>.Default.Equals(_someValue, other._someValue);

        public override bool Equals(object? other)
            => other is Option<T> m && Equals(m);

        public override int GetHashCode() =>
            IsSome ? EqualityComparer<T>.Default.GetHashCode(_someValue) : 0;

        public static bool operator ==(Option<T> first, Option<T> second) =>
            first.Equals(second);

        public static bool operator !=(Option<T> first, Option<T> second) =>
            !first.Equals(second);

        public override string ToString() => IsSome ? $"Some({_someValue})" : "None";
    }

    public static class Option
    {
        public static Option<T> Some<T>(T value) => Option<T>.Some(value);
        public static Option<T> None<T>() => Option<T>.None;

        public static bool IsNone<T>(Option<T> value) => !value.IsSome;

        public static Option<TA> Pure<TA>(TA value) => Option<TA>.Some(value);
        
        public static bool IsSome<TA>(Option<TA> o) => o.IsSome;

        public static Option<T> Cond<T>(bool cond, T value) => cond ? Option<T>.Some(value) : Option<T>.None;
    }

}