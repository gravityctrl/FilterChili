using System.Numerics;
using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Resolvers.ExpressionFactories;

internal class RangeExpressionFactory<TValue> where TValue : IMinMaxValue<TValue>, IComparisonOperators<TValue, TValue, bool>
{
    private readonly Expression _body;
    private readonly ParameterExpression[] _parameters;

    public RangeExpressionFactory(Expression body, params ParameterExpression[] parameters)
    {
        _body = body;
        _parameters = parameters;
    }

    public Option<Expression<Func<TParameter, bool>>> Create<TParameter>(RangeFilter<TValue> filter)
    {
        if (TValue.MinValue < filter.Min && TValue.MaxValue > filter.Max)
        {
            var minConstant = Expression.Constant(filter.Min);
            var maxConstant = Expression.Constant(filter.Max);
            var greaterThanExpression = Expression.GreaterThanOrEqual(_body, minConstant);
            var lessThanExpression = Expression.LessThanOrEqual(_body, maxConstant);
            var andExpression = Expression.AndAlso(greaterThanExpression, lessThanExpression);
            return Expression.Lambda<Func<TParameter, bool>>(andExpression, _parameters);
        }

        if (TValue.MaxValue > filter.Max)
        {
            var maxConstant = Expression.Constant(filter.Max);
            var expression = Expression.LessThanOrEqual(_body, maxConstant);
            return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
        }

        // ReSharper disable once InvertIf
        if (TValue.MinValue < filter.Min)
        {
            var minConstant = Expression.Constant(filter.Min);
            var expression = Expression.GreaterThanOrEqual(_body, minConstant);
            return Expression.Lambda<Func<TParameter, bool>>(expression, _parameters);
        }
        
        return Option.None();
    }
}