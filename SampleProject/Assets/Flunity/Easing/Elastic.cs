using System;

namespace Flunity.Easing
{
	/// <summary>
	/// Elastic equations
	/// </summary>
	public class Elastic
	{
		public static EasyFunction easeIn = EaseInWith();
		public static EasyFunction easeOut = EaseOutWith();
		public static EasyFunction easeInOut = EaseInOutWith();

		public static EasyFunction EaseInWith(double a = 0.1, double p = 0.4)
		{
			return delegate(double k)
			       {
			       	if (k == 0)
			       		return 0;

			       	if (k == 1)
			       		return 1;

			       	if (p == 0)
			       		p = 0.3;

			       	double s;

			       	if (a == 0 || a < 1)
			       	{
			       		a = 1;
			       		s = p / 4;
			       	}
			       	else
			       	{
			       		s = p / (2 * Math.PI) * Math.Asin(1 / a);
			       	}

			       	return -(a * Math.Pow(2, 10 * (k -= 1)) * Math.Sin((k - s) * (2 * Math.PI) / p));
			       };
		}

		public static EasyFunction EaseOutWith(double a = 0.1, double p = 0.4)
		{
			return delegate(double k)
			       {
			       	if (k == 0)
			       		return 0;

			       	if (k == 1)
			       		return 1;

			       	if (p == 0)
			       		p = 0.3;

			       	double s;

			       	if (a == 0 || a < 1)
			       	{
			       		a = 1;
			       		s = p / 4;
			       	}
			       	else
			       	{
			       		s = p / (2 * Math.PI) * Math.Asin(1 / a);
			       	}

			       	return (a * Math.Pow(2, -10 * k) * Math.Sin((k - s) * (2 * Math.PI) / p) + 1);
			       };
		}

		public static EasyFunction EaseInOutWith(double a = 0.1, double p = 0.4)
		{
			return delegate(double k)
			       {
			       	if (k == 0)
			       		return 0;

			       	if (k == 1)
			       		return 1;

			       	if (p == 0)
			       		p = 0.3;

			       	double s;

			       	if (a == 0 || a < 1)
			       	{
			       		a = 1;
			       		s = p / 4;
			       	}
			       	else
			       	{
			       		s = p / (2 * Math.PI) * Math.Asin(1 / a);
			       	}

			       	return (k *= 2) < 1
			       	       	? -0.5 * (a * Math.Pow(2, 10 * (k -= 1)) * Math.Sin((k - s) * (2 * Math.PI) / p))
			       	       	: a * Math.Pow(2, -10 * (k -= 1)) * Math.Sin((k - s) * (2 * Math.PI) / p) * .5 + 1;
			       };
		}
	}
}