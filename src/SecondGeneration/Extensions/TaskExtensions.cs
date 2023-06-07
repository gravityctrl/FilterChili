// ReSharper disable once CheckNamespace
namespace System.Threading.Tasks;

internal static class TaskExtensions
{
    public static Task<T[]> WhenAll<T>(this IEnumerable<Task<T>> tasks)
    {
        return Task.WhenAll(tasks);
    }
}