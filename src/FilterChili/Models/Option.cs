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
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Models
{
    internal abstract class Option
    {
        [NotNull]
        public static Option<T> None<T>()
        {
            return new None<T>();
        }

        [NotNull]
        public static Option<T> Some<T>([CanBeNull] T value)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            return value == null ? throw new ArgumentNullException(nameof(value)) : new Some<T>(value);
        }

        [NotNull]
        public static Option<T> Maybe<T>([CanBeNull] T value)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            return value == null ? (Option<T>)new None<T>() : new Some<T>(value);
        }
    }

    // ReSharper disable once UnusedTypeParameter
    internal abstract class Option<T> : Option
    {
        [NotNull]
        public static implicit operator Option<T>(T value)
        {
            return Maybe(value);
        }
    }

    internal sealed class Some<T> : Option<T>
    {
        public T Value { get; }

        public Some(T value)
        {
            Value = value;
        }
    }

    internal sealed class None<T> : Option<T> {}

    internal static class OptionExtensions
    {
        public static bool TryGetValue<T>([CanBeNull] this Option<T> option, out T value)
        {
            if (option is Some<T> some)
            {
                value = some.Value;
                return true;
            }

            value = default;
            return false;
        }

        [ItemNotNull]
        public static IEnumerable<T> SelectValues<T>([NotNull] this IEnumerable<Option<T>> options)
        {
            foreach (var option in options)
            {
                if (option.TryGetValue(out var value))
                {
                    yield return value;
                }
            }
        }
    }
}
