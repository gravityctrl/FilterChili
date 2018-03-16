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
using GravityCTRL.FilterChili.Comparison;
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili
{
    public class ByteComparisonResolver<TSource> : ComparisonResolver<TSource, byte>
    {
        internal ByteComparisonResolver(Comparer<TSource, byte> comparer, Expression<Func<TSource, byte>> selector) : base(comparer, selector) { }
    }

    public class CharComparisonResolver<TSource> : ComparisonResolver<TSource, char>
    {
        internal CharComparisonResolver(Comparer<TSource, char> comparer, Expression<Func<TSource, char>> selector) : base(comparer, selector) { }
    }

    public class DecimalComparisonResolver<TSource> : ComparisonResolver<TSource, decimal>
    {
        internal DecimalComparisonResolver(Comparer<TSource, decimal> comparer, Expression<Func<TSource, decimal>> selector) : base(comparer, selector) { }
    }

    public class DoubleComparisonResolver<TSource> : ComparisonResolver<TSource, double>
    {
        internal DoubleComparisonResolver(Comparer<TSource, double> comparer, Expression<Func<TSource, double>> selector) : base(comparer, selector) { }
    }

    public class FloatComparisonResolver<TSource> : ComparisonResolver<TSource, float>
    {
        internal FloatComparisonResolver(Comparer<TSource, float> comparer, Expression<Func<TSource, float>> selector) : base(comparer, selector) { }
    }

    public class IntComparisonResolver<TSource> : ComparisonResolver<TSource, int>
    {
        internal IntComparisonResolver(Comparer<TSource, int> comparer, Expression<Func<TSource, int>> selector) : base(comparer, selector) { }
    }

    public class LongComparisonResolver<TSource> : ComparisonResolver<TSource, long>
    {
        internal LongComparisonResolver(Comparer<TSource, long> comparer, Expression<Func<TSource, long>> selector) : base(comparer, selector) { }
    }

    public class SByteComparisonResolver<TSource> : ComparisonResolver<TSource, sbyte>
    {
        internal SByteComparisonResolver(Comparer<TSource, sbyte> comparer, Expression<Func<TSource, sbyte>> selector) : base(comparer, selector) { }
    }

    public class ShortComparisonResolver<TSource> : ComparisonResolver<TSource, short>
    {
        internal ShortComparisonResolver(Comparer<TSource, short> comparer, Expression<Func<TSource, short>> selector) : base(comparer, selector) { }
    }

    public class UIntComparisonResolver<TSource> : ComparisonResolver<TSource, uint>
    {
        internal UIntComparisonResolver(Comparer<TSource, uint> comparer, Expression<Func<TSource, uint>> selector) : base(comparer, selector) { }
    }

    public class ULongComparisonResolver<TSource> : ComparisonResolver<TSource, ulong>
    {
        internal ULongComparisonResolver(Comparer<TSource, ulong> comparer, Expression<Func<TSource, ulong>> selector) : base(comparer, selector) { }
    }

    public class UShortComparisonResolver<TSource> : ComparisonResolver<TSource, ushort>
    {
        internal UShortComparisonResolver(Comparer<TSource, ushort> comparer, Expression<Func<TSource, ushort>> selector) : base(comparer, selector) { }
    }
}
