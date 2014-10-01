using System;

namespace Flunity.Easing
{
	/// <summary>
	/// Exponential equations
	/// </summary>
	public class Expo
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return k == 0
					   ? 0
					   : Math.Pow(2, 10 * (k - 1));
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			if (k == 0)
				return 0;

			if (k == 1)
				return 1;

			return (k *= 2) < 1
					   ? 0.5 * Math.Pow(2, 10 * (k - 1))
					   : 0.5 * (-Math.Pow(2, -10 * (k - 1)) + 2);
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return k == 1
					   ? 1
					   : -Math.Pow(2, -10 * k) + 1;
		};
	}
}