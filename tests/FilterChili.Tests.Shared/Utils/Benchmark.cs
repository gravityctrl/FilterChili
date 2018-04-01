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
using System.Diagnostics;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.Shared.Utils
{
    public static class Benchmark
    {
        [NotNull]
        public static BenchmarkResult Measure([NotNull] Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();
            return new BenchmarkResult(stopwatch.Elapsed);
        }

        [NotNull]
        public static BenchmarkResult<T> Measure<T>([NotNull] Func<T> function)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var result = function();

            stopwatch.Stop();
            return new BenchmarkResult<T>(stopwatch.Elapsed, result);
        }
    }
}
