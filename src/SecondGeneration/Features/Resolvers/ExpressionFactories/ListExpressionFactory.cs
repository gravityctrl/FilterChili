using SecondGeneration.Models.Filters;

namespace SecondGeneration.Features.Resolvers.ExpressionFactories;

internal class ListExpressionFactory<TValue>
{
    private readonly Expression _body;
    private readonly ParameterExpression[] _parameters;

    public ListExpressionFactory(Expression body, params ParameterExpression[] parameters)
    {
        _body = body;
        _parameters = parameters;
    }

    public Option<Expression<Func<TParameter, bool>>> Create<TParameter>(ListFilter<TValue> filter)
    {
        var expression = filter
            .Values
            .Select(item => Expression.Constant(item))
            .Select(expression => Expression.Equal(expression, _body))
            .Or();
        
        return expression.TryGetValue(out var value)
            ? Expression.Lambda<Func<TParameter, bool>>(value, _parameters)
            : Option.None();
    }
}