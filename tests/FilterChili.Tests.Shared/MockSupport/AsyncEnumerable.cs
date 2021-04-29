using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.Shared.MockSupport
{
    internal sealed class AsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        IQueryProvider IQueryable.Provider => new AsyncQueryProvider<T>(this);

        public AsyncEnumerable([NotNull] IEnumerable<T> enumerable)
            : base(enumerable) {}

        public AsyncEnumerable([NotNull] Expression expression)
            : base(expression) {}

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = new())
        {
            return new AsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}