using System.Collections.Generic;
using System;
using System.Text;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class EnumerableUtil
	{
		public static string JoinStrings<T>(this IEnumerable<T> source, Func<T, string> projection, string separator = ",")
		{
			var builder = new StringBuilder();
			bool first = true;
			foreach (T element in source)
			{
				if (first)
				{
					first = false;
				}
				else
				{
					builder.Append(separator);
				}
				builder.Append(projection(element));
			}
			return builder.ToString();
		}
		
		public static string JoinStrings<T>(this IEnumerable<T> source, string separator = ",")
		{
			return JoinStrings(source, t => t.ToString(), separator);
		}

		/// <summary>
		/// Wraps Enumerator by Enumerable
		/// </summary>
		public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator)
		{
			while (enumerator.MoveNext())
			{
				yield return enumerator.Current;
			}
		}
		 
	}
}