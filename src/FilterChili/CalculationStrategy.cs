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
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili
{
    /// <summary>
    /// Specifies in which detail the filterable values are reported.
    /// </summary>
    [Flags]
    public enum CalculationStrategy
    {
        [UsedImplicitly]
        SelectedValues = 0b00,

        [UsedImplicitly]
        SelectableValues = 0b01,

        [UsedImplicitly]
        AvailableValues = 0b10,

        [UsedImplicitly]
        Full = 0b11
    }
}
