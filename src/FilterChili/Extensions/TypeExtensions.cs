// This file is part of FilterChili.
// Copyright © 2017 Sebastian Krogull.
// 
// FilterChili is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// FilterChili is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with FilterChili. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Text;

namespace GravityCTRL.FilterChili.Extensions
{
    internal static class TypeExtensions
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
