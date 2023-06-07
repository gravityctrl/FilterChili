// ReSharper disable once CheckNamespace
namespace System.Linq.Expressions;

internal static class ExpressionExtensions
{
    public static Option<Expression> Or(this IEnumerable<Expression> expressions)
    {
        return CreateExpression(Expression.Or, expressions);
    }
    
    public static Option<Expression> And(this IEnumerable<Expression> expressions)
    {
        return CreateExpression(Expression.AndAlso, expressions);
    }
    
    public static string Name(this LambdaExpression selector)
    {
        return ExtractExpressionName(selector.Body).Else(Guid.NewGuid().ToString());
    }
    
    private static Option<Expression> CreateExpression(
        Func<Expression, Expression, BinaryExpression> binaryExpression,
        IEnumerable<Expression> expressions)
    {
        var expressionList = expressions as IList<Expression> ?? expressions.ToList();

        if (expressionList.Count == 0)
        {
            return Option.None();
        }

        var expression = expressionList.First();
        if (expressionList.Count == 1)
        {
            return Option.Some(expression);
        }

        for (var index = 1; index < expressionList.Count; index++)
        {
            expression = binaryExpression(expressionList[index], expression);
        }
        
        return Option.Some(expression);
    }

    private static Option<string> ExtractExpressionName(Expression expression)
    {
        while (true)
        {
            switch (expression)
            {
                case MemberExpression memberExpression:
                {
                    return memberExpression.Member.Name;
                }
                case MethodCallExpression {Object: { } methodCallObject}:
                {
                    expression = methodCallObject;
                    continue;
                }
                default:
                {
                    return Option.None();
                }
            }
        }
    }
}