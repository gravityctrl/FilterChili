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

using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.TestSupport.Models
{
    public class GenericSource
    {
        [UsedImplicitly]
        public byte Byte { get; set; }

        [UsedImplicitly]
        public char Char { get; set; }

        [UsedImplicitly]
        public decimal Decimal { get; set; }

        [UsedImplicitly]
        public double Double { get; set; }

        [UsedImplicitly]
        public float Float { get; set; }

        [UsedImplicitly]
        public int Int { get; set; }

        [UsedImplicitly]
        public long Long { get; set; }

        [UsedImplicitly]
        public sbyte SByte { get; set; }

        [UsedImplicitly]
        public short Short { get; set; }

        [UsedImplicitly]
        public string String { get; set; }

        [UsedImplicitly]
        public uint UInt { get; set; }

        [UsedImplicitly]
        public ulong ULong { get; set; }

        [UsedImplicitly]
        public ushort UShort { get; set; }
    }
}
