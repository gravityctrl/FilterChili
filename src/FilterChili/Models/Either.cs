// This file is part of FilterChili.
// Copyright © 2017 Sebastian Krogull.
// 
// FilterChili is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// FilterChili is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with FilterChili. If not, see <http://www.gnu.org/licenses/>.

using System;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Models
{
    internal abstract class Either
    {
        [NotNull]
        public static Either<TLeft, TRight> Left<TLeft, TRight>(TLeft left)
        {
            return new Left<TLeft, TRight>(left);
        }

        [NotNull]
        public static Either<TLeft, TRight> Right<TLeft, TRight>(TRight right)
        {
            return new Right<TLeft, TRight>(right);
        }
    }

    internal abstract class Either<TLeft, TRight> : Either
    {
        [NotNull]
        public static implicit operator Either<TLeft, TRight>(TLeft value)
        {
            if (typeof(TLeft) == typeof(TRight))
            {
                throw new InvalidOperationException();
            }

            return new Left<TLeft, TRight>(value);
        }

        [NotNull]
        public static implicit operator Either<TLeft, TRight>(TRight value)
        {
            if (typeof(TLeft) == typeof(TRight))
            {
                throw new InvalidOperationException();
            }

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

        public static TResult Match<TLeft, TRight, TResult>(this Either<TLeft, TRight> either, Func<TLeft, TResult> onLeft, Func<TRight, TResult> onRight)
        {
            switch (either)
            {
                case Left<TLeft, TRight> left:
                {
                    return onLeft(left.Value);
                }
                case Right<TLeft, TRight> right:
                {
                    return onRight(right.Value);
                }
                default:
                {
                    return default;
                }
            }
        }
    }
}
