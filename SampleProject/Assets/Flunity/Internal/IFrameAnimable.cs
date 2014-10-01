namespace Flunity.Internal
{
	/// <summary>
	/// All objects which implements IFrameAnimable can be animated.
	/// See FrameAnimationExt.
	/// </summary>
	public interface IFrameAnimable
	{
		int currentFrame { get; set; }
		int totalFrames { get; }
		FrameAnimation animation { get;}
	}
}
