namespace SecondGeneration.Extensions;

internal static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (var item in items)
        {
            action(item);
        }
    }

    public static IEnumerable<T> Append<T>(this T value, IEnumerable<T> values)
    {
        yield return value;
        foreach (var item in values)
        {
            yield return item;
        }
    }
}