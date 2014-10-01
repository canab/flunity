
namespace Flunity.Easing
{
	/// <summary>
	/// 4-th power equations
	/// </summary>
	public class Quart
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return k * k * k * k;
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			return (k *= 2) < 1
					? 0.5 * k * k * k * k
					: -0.5 * ((k -= 2) * k * k * k - 2);
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return -(--k * k * k * k - 1);
		};

	}
}