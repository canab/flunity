
namespace Flunity.Easing
{
	/// <summary>
	/// Quadratic equations
	/// </summary>
	public class Quad
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return k * k;
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			return (k *= 2) < 1
					? 0.5 * k * k
					: -0.5 * (--k * (k - 2) - 1);
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return -k * (k - 2);
		};
	}
}