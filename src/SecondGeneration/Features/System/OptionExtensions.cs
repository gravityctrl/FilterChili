// ReSharper disable once CheckNamespace
namespace System;

internal static class OptionExtensions
{
    public static bool TryGetValue<T>(this Option<T>? option, out T value)
    {
        if (option is Some<T> some)
        {
            value = some.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public static IEnumerable<T> Select<TIn, T>(this IEnumerable<TIn> elements, Func<TIn, Option<T>> select)
    {
        foreach (var element in elements)
        {
            var option = select(element);
            if (option.TryGetValue(out var value))
            {
                yield return value;
            }
        }
    }

    public static T Else<T>(this Option<T> option, T fallback)
    {
        return option.TryGetValue(out var value)
            ? value
            : fallback;
    }

    public static Option<TResult> Map<T, TResult>(this Option<T> option, Func<T, Option<TResult>> map)
    {
        return option.TryGetValue(out var value)
            ? map(value)
            : Option.None();
    }
}