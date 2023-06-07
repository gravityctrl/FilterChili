using System.Reflection;
using SecondGeneration.Features.Descriptors;
using MatchType = SecondGeneration.Models.Enums.MatchType;

namespace SecondGeneration.Features.Resolvers.ExpressionFactories;

internal class EnumerableExpressionFactory<TSource, TValue>
{
    private readonly MultiFilterProperty<TSource, TValue> _filterProperty;

    public EnumerableExpressionFactory(MultiFilterProperty<TSource, TValue> filterProperty)
    {
        _filterProperty = filterProperty;
    }

    public Option<Expression<Func<TSource, bool>>> Create(Option<Expression<Func<TValue, bool>>> predicateOption)
    {
        return predicateOption.TryGetValue(out var predicate)
            ? BuildEnumerableExpression(predicate)
            : Option.None();
    }

    private Option<Expression<Func<TSource, bool>>> BuildEnumerableExpression(Expression<Func<TValue, bool>> predicate)
    {
        var selector = _filterProperty.Selector;
        var method = GetMethod(_filterProperty);
        var methodCall = Expression.Call(method, selector.Body, predicate);
        return Expression.Lambda<Func<TSource, bool>>(methodCall, selector.Parameters);
    }

    private static MethodInfo GetMethod(MultiFilterProperty<TSource, TValue> filterProperty) => filterProperty.MatchType switch
    {
        MatchType.Any => Any,
        MatchType.All => All,
        _ => throw new ArgumentException(nameof(MatchType))
    };

    private static MethodInfo Any => typeof(Enumerable)
        .GetMethods()
        .Single(method
            => method.Name == nameof(Enumerable.Any)
            && method.GetParameters().Length == 2
        )
        .MakeGenericMethod(typeof(TValue));
 
    private static MethodInfo All => typeof(Enumerable)
        .GetMethods()
        .Single(method
            => method.Name == nameof(Enumerable.All)
            && method.GetParameters().Length == 2
        )
        .MakeGenericMethod(typeof(TValue));
}