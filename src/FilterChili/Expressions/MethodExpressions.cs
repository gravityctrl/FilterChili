using System.Reflection;

namespace GravityCTRL.FilterChili.Expressions
{
    public static class MethodExpressions
    {
        public static readonly MethodInfo StringContainsExpression = typeof(string).GetMethod("Contains", new[] { typeof(string) });
    }
}
