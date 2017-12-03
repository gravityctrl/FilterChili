using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GravityCTRL.FilterChili.Tests.Utils
{
    public static class Benchmark
    {
        public static async Task<TimeSpan> Measure(Func<Task> action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            await action();

            stopwatch.Stop();
            return stopwatch.Elapsed;
        }
    }
}
