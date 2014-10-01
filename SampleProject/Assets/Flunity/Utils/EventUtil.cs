using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class EventUtil
	{
		/// <summary>
		/// Invokes action if action is not null
		/// </summary>
		public static void Dispatch(this Action target)
		{
			if (target != null)
				target();
		}
		
		/// <summary>
		/// Invokes action with one argument if action is not null
		/// </summary>
		public static void Dispatch<T>(this Action<T> target, T param)
		{
			if (target != null)
				target(param);
		}

		/// <summary>
		/// Invokes action with two arguments if action is not null
		/// </summary>
		public static void Dispatch<T1, T2>(this Action<T1, T2> target, T1 param1, T2 param2)
		{
			if (target != null)
				target(param1, param2);
		}
	}
}
