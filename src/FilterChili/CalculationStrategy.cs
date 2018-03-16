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

namespace GravityCTRL.FilterChili
{
    /// <summary>
    /// Specifies in which detail the filterable values are reported.
    /// </summary>
    public enum CalculationStrategy
    {
        /// <summary>
        /// Provides all information on the filterable values per filtered property.
        /// By Choosing this <see cref="CalculationStrategy"/> a set over all existing property values will be created.
        /// This strategy also creates a set of values, that can currently be selected using the filter.
        /// </summary>
        [UsedImplicitly]
        Full = 0,

        /// <summary>
        /// Provides all information on the filterable values per filtered property.
        /// By Choosing this <see cref="CalculationStrategy"/> a set over all existing property values will be created.
        /// This strategy does not create a set of values, that can currently be selected using the filter.
        /// </summary>
        [UsedImplicitly]
        WithoutSelectableValues = 1
    }
}
