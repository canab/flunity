using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class MathUtil
	{
		public const double PI = Math.PI;
		public const double DOUBLE_PI = 2 * Math.PI;

		public static byte ClampByte(this byte value, byte a, byte b)
		{
			var minValue = a < b ? a : b;
			var maxValue = a > b ? a : b;
			
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;
			return value;
		}

		public static byte ClampByte(this float value, byte a, byte b)
		{
			var minValue = a < b ? a : b;
			var maxValue = a > b ? a : b;
			
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;

			return RoundToByte(value);
		}

		public static int ClampInt(this int value, int a, int b)
		{
			var minValue = a < b ? a : b;
			var maxValue = a > b ? a : b;
			
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;
			return value;
		}

		public static int ClampInt(this float value, int a, int b)
		{
			var minValue = a < b ? a : b;
			var maxValue = a > b ? a : b;
			
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;

			return RoundToInt(value);
		}

		public static int ClampInt(this double value, int a, int b)
		{
			return ClampInt((float) value, a, b);
		}

		public static float ClampFloat(this float value, float a, float b)
		{
			var minValue = a < b ? a : b;
			var maxValue = a > b ? a : b;
			
			if (value < minValue)
				return minValue;
			if (value > maxValue)
				return maxValue;

			return value;
		}

		public static float ClampFloat(this double value, float a, float b)
		{
			return ClampFloat((float) value, a, b);
		}

		public static float ClampUnit(this float value)
		{
			if (value < 0)
				return 0;

			if (value > 1)
				return 1;

			return value;
		}
		
		public static bool IsBetween(this float value, float a, float b)
		{
			return value >= a && value <= b;
		}
		
		public static bool IsBetween(this double value, double a, double b)
		{
			return value >= a && value <= b;
		}

		/// <summary>
		/// Calculates angle difference 
		/// </summary>
		/// <returns>difference in range [-PI...PI]</returns>
		/// <param name="angle1">radians</param>
		/// <param name="angle2">radians</param>
		public static double AngleDiff(double angle1, double angle2)
		{
			angle1 %= DOUBLE_PI;
			angle2 %= DOUBLE_PI;

			if (angle1 < 0)
				angle1 += DOUBLE_PI;

			if (angle2 < 0)
				angle2 += DOUBLE_PI;

			var diff = angle2 - angle1;

			if (diff < -PI)
				diff += DOUBLE_PI;

			if (diff > PI)
				diff -= DOUBLE_PI;

			return diff;
		}

		public static float Square(this float value)
		{
			return value * value;
		}

		public static float Interpolate(float from, float to, float distance)
		{
			return from + distance * (to - from);
		}

		public static int RoundToInt(this double value)
		{
			return (int)Math.Round(value);
		}
		
		public static int RoundToInt(this float value)
		{
			return (int)Math.Round(value);
		}

		public static byte RoundToByte(this float value)
		{
			return (byte)Math.Round(value);
		}

		public static float Min(float x1, float x2, float x3, float x4)
		{
			return Math.Min(Math.Min(x1, x2), Math.Min(x3, x4));
		}
		
		public static float Max(float x1, float x2, float x3, float x4)
		{
			return Math.Max(Math.Max(x1, x2), Math.Max(x3, x4));
		}

		public static float Min(float x1, float x2, float x3)
		{
			return Math.Min(Math.Min(x1, x2), x3);
		}
		
		public static float Max(float x1, float x2, float x3)
		{
			return Math.Max(Math.Max(x1, x2), x3);
		}

	}
}
