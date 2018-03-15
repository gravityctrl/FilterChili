using System;
using System.Text;

namespace GravityCTRL.FilterChili.Utils
{
    internal static class TypeUtils
    {
        private const string START_DELIMITER = "<";
        private const string END_DELIMITER = ">";
        private const string SEPARATOR = ",";
        private const string GENERIC_MARKER = "`";

        public static string GetFormattedName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var sb = new StringBuilder();

            sb.Append(type.Name.Substring(0, type.Name.LastIndexOf(GENERIC_MARKER, StringComparison.Ordinal)));

            var result = START_DELIMITER;
            foreach (var genericArgument in type.GetGenericArguments())
            {
                var createSeparator = result != START_DELIMITER;
                var seperator = createSeparator ? SEPARATOR : string.Empty;
                result = $"{result}{seperator}{genericArgument.GetFormattedName()}";
            }

            sb.Append(result);
            sb.Append(END_DELIMITER);

            return sb.ToString();
        }
    }
}
