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
using GravityCTRL.FilterChili.Resolvers;

namespace GravityCTRL.FilterChili.Behaviors
{
    internal class ReplaceNameBehavior<TSource, TSelector> : IBehavior<TSource, TSelector> where TSelector : IComparable
    {
        private readonly string _name;

        public ReplaceNameBehavior(string name)
        {
            _name = name;
        }

        public void Apply<TDomainResolver>(TDomainResolver resolver) where TDomainResolver : DomainResolver<TDomainResolver, TSource, TSelector>
        {
            resolver.Name = _name;
        }
    }
}
