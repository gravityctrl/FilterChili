using System;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Models
{
    internal abstract class Either
    {
        [NotNull]
        public static Either<TLeft, TRight> Some<TLeft, TRight>(TLeft left)
        {
            return new Left<TLeft, TRight>(left);
        }

        [NotNull]
        public static Either<TLeft, TRight> Some<TLeft, TRight>(TRight right)
        {
            return new Right<TLeft, TRight>(right);
        }
    }

    internal abstract class Either<TLeft, TRight> : Either
    {
        [NotNull]
        public static implicit operator Either<TLeft, TRight>(TLeft value)
        {
            return new Left<TLeft, TRight>(value);
        }

        [NotNull]
        public static implicit operator Either<TLeft, TRight>(TRight value)
        {
            return new Right<TLeft, TRight>(value);
        }
    }

    internal sealed class Left<TLeft, TRight> : Either<TLeft, TRight>
    {
        public TLeft Value { get; }

        public Left(TLeft value)
        {
            Value = value;
        }
    }

    internal sealed class Right<TLeft, TRight> : Either<TLeft, TRight>
    {
        public TRight Value { get; }

        public Right(TRight value)
        {
            Value = value;
        }
    }

    internal static class EitherExtensions
    {
        public static bool TryGetLeft<TLeft, TRight>([NotNull] this Either<TLeft, TRight> either, out TLeft value)
        {
            if (either is Left<TLeft, TRight> left)
            {
                value = left.Value;
                return true;
            }

            value = default;
            return false;
        }

        public static bool TryGetRight<TLeft, TRight>([NotNull] this Either<TLeft, TRight> either, out TRight value)
        {
            if (either is Right<TLeft, TRight> right)
            {
                value = right.Value;
                return true;
            }

            value = default;
            return false;
        }

        public static void Match<TLeft, TRight>(this Either<TLeft, TRight> either, Action<TLeft> onLeft, Action<TRight> onRight)
        {
            switch (either)
            {
                case Left<TLeft, TRight> left:
                {
                    onLeft(left.Value);
                    break;
                }
                case Right<TLeft, TRight> right:
                {
                    onRight(right.Value);
                    break;
                }
            }
        }
    }
}
