using System;

namespace Flunity.Easing
{
	/// <summary>
	/// Sinus equations
	/// </summary>
	public class Sine
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return 1 - Math.Cos(k * (Math.PI / 2));
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			return -(Math.Cos(Math.PI * k) - 1) / 2;
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return Math.Sin(k * (Math.PI / 2));
		};
	}
}