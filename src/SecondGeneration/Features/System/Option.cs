// ReSharper disable once CheckNamespace
namespace System;

internal static class Option
{
    public static None None()
    {
        return new None();
    }
    
    public static Option<T> None<T>()
    {
        return new None<T>();
    }

    public static Option<T> Some<T>(T? value)
    {
        return value == null
            ? throw new ArgumentNullException(nameof(value))
            : new Some<T>(value);
    }

    public static Option<T> Maybe<T>(T? value)
    {
        return value is not null
            ? new Some<T>(value)
            : new None<T>();
    }
}

internal abstract class Option<T>
{
    public static implicit operator Option<T>(None _)
    {
        return new None<T>();
    }
    
    public static implicit operator Option<T>(T? value)
    {
        return Option.Maybe(value);
    }
}

internal sealed class Some<T> : Option<T>
{
    public T Value { get; }

    public Some(T value)
    {
        Value = value;
    }
}

internal readonly struct None {}
internal sealed class None<T> : Option<T> {}
