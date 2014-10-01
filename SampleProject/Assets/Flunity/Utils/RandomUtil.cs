using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class RandomUtil
	{
		private static readonly Random _random = new Random();

		public static float RandomFloat(double min, double max)
		{
			return (float) (min + _random.NextDouble()*(max - min));
		}
		
		public static float RandomFloat(float min, float max)
		{
			return (float) (min + _random.NextDouble()*(max - min));
		}
		
		public static int RandomSign()
		{
			return _random.NextDouble() < 0.5 ? -1 : 1;
		}
		
		public static bool RandomBool()
		{
			return _random.NextDouble() < 0.5;
		}
		
		public static int RandomInt(int min, int max)
		{
			return min + (int) Math.Round(_random.NextDouble() * (max - min));
		}
		
		public static float RandomFloat()
		{
			return (float) _random.NextDouble();
		}
		
	}
}
