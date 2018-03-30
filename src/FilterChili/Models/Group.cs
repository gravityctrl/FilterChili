﻿// This file is part of FilterChili.
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
    public sealed class Group<TGroupIdentifier, TValue> where TGroupIdentifier : IComparable where TValue : IComparable
    {
        [UsedImplicitly]
        public TGroupIdentifier Identifier { get; internal set; }

        [UsedImplicitly]
        public IReadOnlyList<Item<TValue>> Values { get; internal set; }
    }
}