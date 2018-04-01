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

namespace GravityCTRL.FilterChili.Tests.Shared.Utils
{
    public class BenchmarkResult
    {
        public TimeSpan ElapsedTime { get; }

        internal BenchmarkResult(TimeSpan elapsedTime)
        {
            ElapsedTime = elapsedTime;
        }
    }

    public sealed class BenchmarkResult<T> : BenchmarkResult
    {
        public T Result { get; }

        internal BenchmarkResult(TimeSpan elapsedTime, T result) : base(elapsedTime)
        {
            Result = result;
        }
    }
}