
namespace Flunity.Easing
{
	/// <summary>
	/// Cubic equations
	/// </summary>
	public class Cubic
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return k * k * k;
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			return (k *= 2) < 1
					? 0.5 * k * k * k
					: 0.5 * ((k -= 2) * k * k + 2);
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return --k * k * k + 1;
		};
	}
}