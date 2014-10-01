
namespace Flunity.Easing
{
	/// <summary>
	/// Linear equations
	/// </summary>
	public class Linear
	{
		public static EasyFunction easeNone = delegate(double k)
		{
			return k;
		};
	}
}