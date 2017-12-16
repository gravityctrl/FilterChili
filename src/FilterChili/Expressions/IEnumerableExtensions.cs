using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GravityCTRL.FilterChili.Expressions
{
    public static class IEnumerableExtensions
    {
        public static Expression Or(this IEnumerable<Expression> expressions)
        {
            var expressionList = expressions as IList<Expression> ?? expressions.ToList();

            if (expressionList.Count == 0)
            {
                return null;
            }

            var expression = expressionList[0];
            if (expressionList.Count == 1)
            {
                return expression;
            }

            for (var index = 1; index < expressionList.Count; index++)
            {
                expression = Expression.Or(expression, expressionList[index]);
            }

            return expression;
        }
    }
}
