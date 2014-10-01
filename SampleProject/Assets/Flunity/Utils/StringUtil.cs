using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class StringUtil
	{
		public static string Format(this string target, params object[] args)
		{
			return target == null ? null : string.Format(target, args);
		}

		public static bool IsNullOrEmpty(this string target)
		{
			return string.IsNullOrEmpty(target);
		}

		public static bool IsNotEmpty(this string target)
		{
			return !string.IsNullOrEmpty(target);
		}
		
		public static string[] Split(this string target, string separator)
		{
			return target == null ? null
				: target.Split(new []{separator}, StringSplitOptions.None);
		}

		public static int ToInt(this string target)
		{
			if (target == null)
				return 0;

			int result;
			return Int32.TryParse(target, out result) ? result : 0;
		}
		
		public static string Max(string a, string b)
		{
			return string.CompareOrdinal(a, b) > 0 ? a : b;
		}

		public static bool GraterThen(this string a, string b)
		{
			return string.CompareOrdinal(a, b) > 0;
		}

		public static bool GraterOrEqualsThen(this string a, string b)
		{
			return string.CompareOrdinal(a, b) >= 0;
		}

		public static bool LessThen(this string a, string b)
		{
			return string.CompareOrdinal(a, b) < 0;
		}
		
		public static bool LessOrEqualsThen(this string a, string b)
		{
			return string.CompareOrdinal(a, b) <= 0;
		}

	}
}