using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.Shared.MockSupport
{
    public static class AsyncExtensions
    {
        [NotNull]
        public static IQueryable<T> ToAsyncQueryable<T>([NotNull] this IEnumerable<T> input)
        {
            return new AsyncEnumerable<T>(input).AsQueryable();
        }
    }
}
