using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Talon.Utility
{
	public static class StringUtility
	{
		public static string Join(this IEnumerable<string> seq, string separator)
		{
			StringBuilder sbResult = seq.Intersperse(separator ?? string.Empty).Aggregate(new StringBuilder(), (sb, value) => sb.Append(value));
			return sbResult.ToString();
		}
	}
}