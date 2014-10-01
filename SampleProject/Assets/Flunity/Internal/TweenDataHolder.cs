
using Flunity.Common;

namespace Flunity.Internal
{
	/// <summary>
	/// Holds start/end values of tweened object.
	/// </summary>
	public class TweenDataHolder
	{
		public const int MAX_VALUES = 8;
		
		public static readonly ObjectPool<TweenDataHolder> pool = new ObjectPool<TweenDataHolder>(
			() => new TweenDataHolder());

		public float[] startValue = new float[MAX_VALUES];
		public float[] endValue = new float[MAX_VALUES];

		private TweenDataHolder()
		{}
	}
}
