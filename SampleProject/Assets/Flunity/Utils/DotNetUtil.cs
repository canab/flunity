using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Reflection;
using System.Linq;

namespace Flunity.Utils
{
	/// <summary>
	/// .NET methods which are not implemented in .NET 20
	/// </summary>
	public static class DotNetUtil
	{
		public static void Restart(this Stopwatch timer)		
		{
			timer.Stop();
			timer.Reset();
			timer.Start();
		}

		public static StringBuilder Clear(this StringBuilder builder)		
		{
			builder.Length = 0;
			return builder;
		}

		public static void CopyTo(this Stream input, Stream output)
		{
			// This method exists only in .NET 4 and higher
			
			byte[] buffer = new byte[4 * 1024];
			int bytesRead;
			
			while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
			{
				output.Write(buffer, 0, bytesRead);
			}
		}

		public static FieldInfo[] GetAllFields(this Type type )
		{
			// http://stackoverflow.com/a/1155549/154165
			
			if( type == null )
				return new FieldInfo[0];
			
			const BindingFlags flags =
				BindingFlags.Public	| BindingFlags.NonPublic
				| BindingFlags.Instance | BindingFlags.DeclaredOnly;
			
			return type.GetFields(flags)
				.Concat(GetAllFields(type.BaseType))
				.ToArray();
		}
	}
}

