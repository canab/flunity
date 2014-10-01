using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class ArrayUtil
	{
		public static int IndexOf<T>(this T[] array, T item)
		{
			return Array.IndexOf(array, item);
		}
		
		public static bool Contains<T>(this T[] array, T item)
		{
			return Array.IndexOf(array, item) >= 0;
		}
		
		public static T[] AsArray<T>(this T item)
		{
			return new[] {item};
		}

		public static void Clear<T>(this T[] array)
		{
			Array.Clear(array, 0, array.Length);
		}

		public static int NextIndex<T>(this T[] array, int index)
		{
			return index < array.Length - 1 ? index + 1 : 0;
		}
		
		public static bool ContainsIndex(this Array array, int index)
		{
			return index >= 0 && index < array.Length;
		}
		
		public static void Shuffle<T>(this T[] array)
		{
			int swapCount = array.Length;
			while (swapCount > 1)
			{
				swapCount--;
				int k = RandomUtil.RandomInt(0, swapCount);
				T value = array[k];
				array[k] = array[swapCount];
				array[swapCount] = value;
			}
		}		

		private static void AssertNotEmpty(Array array)
		{
			if (array == null)
				throw new ArgumentNullException("array");

			if (array.Length == 0)
				throw new ArgumentException("Colection is empty");
		}
	}
}
