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

namespace GravityCTRL.FilterChili.Models
{
    public sealed class Range<TValue>
    {
        [UsedImplicitly]
        public TValue Min { get; internal set; }

        [UsedImplicitly]
        public TValue Max { get; internal set; }

        public Range(TValue min, TValue max)
        {
            Min = min;
            Max = max;
        }
    }
}
