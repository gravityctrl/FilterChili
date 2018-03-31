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