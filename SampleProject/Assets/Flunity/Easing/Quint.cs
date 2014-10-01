
namespace Flunity.Easing
{
	/// <summary>
	/// 5-rh power equations
	/// </summary>
	public class Quint
	{
		public static EasyFunction easeIn = delegate(double k)
		{
			return k * k * k * k * k;
		};

		public static EasyFunction easeInOut = delegate(double k)
		{
			return (k *= 2) < 1
					? 0.5 * k * k * k * k * k
					: 0.5 * ((k -= 2) * k * k * k * k + 2);
		};

		public static EasyFunction easeOut = delegate(double k)
		{
			return (k = k - 1) * k * k * k * k + 1;
		};
	}
}