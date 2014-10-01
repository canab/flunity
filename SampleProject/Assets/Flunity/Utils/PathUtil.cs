using System;
using System.IO;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public class PathUtil
	{
		public static string Combine(params string[] parts)
		{
			if (parts.Length == 0)
				return "";

			var result = parts[0];

			for (int i = 1; i < parts.Length; i++)
			{
				result = Path.Combine(result, parts[i]);
			}

			return result.Replace("\\", "/");
		}
	}
}

