using System.Collections.Generic;
using GravityCTRL.FilterChili.Tests.Models;

namespace GravityCTRL.FilterChili.Tests.Extensions
{
    public static class EnumerableExtensions
    {
        public static AsyncEnumerable<T> AsAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            return new AsyncEnumerable<T>(source);
        }
    }
}
