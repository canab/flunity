namespace Flunity.Easing
{
	/// <summary>
	/// Back bouncing equations
	/// </summary>
	public class Back
	{
		public static EasyFunction easeIn = EaseInWith();
		public static EasyFunction easeInOut = EaseInOutWith();
		public static EasyFunction easeOut = EaseOutWith();

		public static EasyFunction EaseInWith(double s = 1.70158)
		{
			return k => k * k * ((s + 1) * k - s);
		}

		public static EasyFunction EaseOutWith(double s = 1.70158)
		{
			return k => (k = k - 1) * k * ((s + 1) * k + s) + 1;
		}

		public static EasyFunction EaseInOutWith(double s = 1.70158)
		{
			s *= 1.525;

			return k => (k *= 2) < 1
			            	? 0.5 * (k * k * ((s + 1) * k - s))
			            	: 0.5 * ((k -= 2) * k * ((s + 1) * k + s) + 2);
		}
	}
}