using System.Numerics;
using SecondGeneration.Models.Enums;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Resolvers.ExpressionFactories;

internal class ComparisonExpressionFactory<TValue> where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly Comparison _comparison;
    private readonly Expression _body;
    private readonly ParameterExpression[] _parameters;

    public ComparisonExpressionFactory(Comparison comparison, Expression body, params ParameterExpression[] parameters)
    {
        _comparison = comparison;
        _body = body;
        _parameters = parameters;
    }

    public Option<Expression<Func<TParameter, bool>>> Create<TParameter>(ComparisonFilter<TValue> filter)
    {
        var value = filter.Value;
        return _comparison switch
        {
            Comparison.LessThan => LessThan<TParameter>(value),
            Comparison.GreaterThan => GreaterThan<TParameter>(value),
            Comparison.LessThanOrEqual => LessThanOrEqual<TParameter>(value),
            Comparison.GreaterThanOrEqual => GreaterThanOrEqual<TParameter>(value),
            _ => throw new ArgumentException(nameof(_comparison))
        };
    }

    private Option<Expression<Func<TParameter, bool>>> LessThan<TParameter>(TValue selectedValue)
    {
        if (TValue.MaxValue <= selectedValue)
        {
            return Option.None();
        }

        var valueConstant = Expression.Constant(selectedValue);
        var expression = Expression.LessThan(_body, valueConstant);
        return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
    }
    
    private Option<Expression<Func<TParameter, bool>>> GreaterThan<TParameter>(TValue selectedValue)
    {
        if (TValue.MinValue >= selectedValue)
        {
            return Option.None();
        }

        var valueConstant = Expression.Constant(selectedValue);
        var expression = Expression.GreaterThan(_body, valueConstant);
        return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
    }
    
    private Option<Expression<Func<TParameter, bool>>> LessThanOrEqual<TParameter>(TValue selectedValue)
    {
        if (TValue.MaxValue <= selectedValue)
        {
            return Option.None();
        }

        var valueConstant = Expression.Constant(selectedValue);
        var expression = Expression.LessThanOrEqual(_body, valueConstant);
        return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
    }
    
    private Option<Expression<Func<TParameter, bool>>> GreaterThanOrEqual<TParameter>(TValue selectedValue)
    {
        if (TValue.MinValue >= selectedValue)
        {
            return Option.None();
        }

        var valueConstant = Expression.Constant(selectedValue);
        var expression = Expression.GreaterThanOrEqual(_body, valueConstant);
        return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
    }
}