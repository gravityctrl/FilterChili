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
using System.Linq.Expressions;
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili
{
    public class ByteRangeResolver<TSource> : RangeResolver<TSource, byte>
    {
        internal ByteRangeResolver(Expression<Func<TSource, byte>> selector) : base(selector, byte.MinValue, byte.MaxValue) {}
    }

    public class CharRangeResolver<TSource> : RangeResolver<TSource, char>
    {
        internal CharRangeResolver(Expression<Func<TSource, char>> selector) : base(selector, char.MinValue, char.MaxValue) { }
    }

    public class DecimalRangeResolver<TSource> : RangeResolver<TSource, decimal>
    {
        internal DecimalRangeResolver(Expression<Func<TSource, decimal>> selector) : base(selector, decimal.MinValue, decimal.MaxValue) { }
    }

    public class DoubleRangeResolver<TSource> : RangeResolver<TSource, double>
    {
        internal DoubleRangeResolver(Expression<Func<TSource, double>> selector) : base(selector, double.MinValue, double.MaxValue) { }
    }

    public class FloatRangeResolver<TSource> : RangeResolver<TSource, float>
    {
        internal FloatRangeResolver(Expression<Func<TSource, float>> selector) : base(selector, float.MinValue, float.MaxValue) { }
    }

    public class IntRangeResolver<TSource> : RangeResolver<TSource, int>
    {
        internal IntRangeResolver(Expression<Func<TSource, int>> selector) : base(selector, int.MinValue, int.MaxValue) { }
    }

    public class LongRangeResolver<TSource> : RangeResolver<TSource, long>
    {
        internal LongRangeResolver(Expression<Func<TSource, long>> selector) : base(selector, long.MinValue, long.MaxValue) { }
    }

    public class SByteRangeResolver<TSource> : RangeResolver<TSource, sbyte>
    {
        internal SByteRangeResolver(Expression<Func<TSource, sbyte>> selector) : base(selector, sbyte.MinValue, sbyte.MaxValue) { }
    }

    public class ShortRangeResolver<TSource> : RangeResolver<TSource, short>
    {
        internal ShortRangeResolver(Expression<Func<TSource, short>> selector) : base(selector, short.MinValue, short.MaxValue) { }
    }

    public class UIntRangeResolver<TSource> : RangeResolver<TSource, uint>
    {
        internal UIntRangeResolver(Expression<Func<TSource, uint>> selector) : base(selector, uint.MinValue, uint.MaxValue) { }
    }

    public class ULongRangeResolver<TSource> : RangeResolver<TSource, ulong>
    {
        internal ULongRangeResolver(Expression<Func<TSource, ulong>> selector) : base(selector, ulong.MinValue, ulong.MaxValue) { }
    }

    public class UShortRangeResolver<TSource> : RangeResolver<TSource, ushort>
    {
        internal UShortRangeResolver(Expression<Func<TSource, ushort>> selector) : base(selector, ushort.MinValue, ushort.MaxValue) { }
    }
}