using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Tests.Shared.MockSupport
{
    internal sealed class AsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public AsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        [CanBeNull] 
        public T Current => _inner.Current;

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }
    }
}