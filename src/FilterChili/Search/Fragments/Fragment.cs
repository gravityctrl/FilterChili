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

namespace GravityCTRL.FilterChili.Search.Fragments
{
    internal abstract class Fragment
    {
        public Guid GroupId { get; set; }

        public string Text { get; }

        [UsedImplicitly]
        public FragmentType Type { get; }

        protected Fragment(FragmentType type, string text)
        {
            Type = type;
            Text = text;
        }
    }
}