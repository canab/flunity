using System;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Methods to run/stop animation on objects which implements IFrameAnimable interface
	/// </summary>
	public static class FrameAnimationExt
	{
		/// <summary>
		/// Stop animation.
		/// </summary>
		public static void Stop(this IFrameAnimable target)
		{
			target.animation.Stop();
		}
		
		/// <summary>
		/// Begin playing forward from current frame.
		/// </summary>
		public static void Play(this IFrameAnimable target)
		{
			target.animation.PlayLoop(1);
		}
		
		/// <summary>
		/// Begin playing backward from current frame.
		/// </summary>
		public static void PlayReverse(this IFrameAnimable target)
		{
			target.animation.PlayLoop(-1);
		}
		
		/// <summary>
		/// Begin playing backward from current frame and stop on the first frame.
		/// PlayCompleted event will be dispatced.
		/// </summary>
		public static void PlayToBegin(this IFrameAnimable target)
		{
			target.animation.PlayTo(0);
		}

		/// <summary>
		/// Begin playing forward from current frame and stop on the last frame.
		/// PlayCompleted event will be dispatced.
		/// </summary>
		public static void PlayToEnd(this IFrameAnimable target)
		{
			target.animation.PlayTo(target.totalFrames - 1);
		}

		/// <summary>
		/// Begin playing from the first frame to the last frame.
		/// PlayCompleted event will be dispatced.
		/// </summary>
		public static void PlayFromBeginToEnd(this IFrameAnimable target)
		{
			target.currentFrame = 0;
			target.PlayToEnd();
		}
		
		/// <summary>
		/// Begin playing from the last frame to the first frame.
		/// PlayCompleted event will be dispatced.
		/// </summary>
		public static void PlayFromEndToBegin(this IFrameAnimable target)
		{
			target.currentFrame = target.totalFrames - 1;
			target.PlayToBegin();
		}
		
		/// <summary>
		/// Begin playing from the current frame to the specified frame.
		/// PlayCompleted event will be dispatced.
		/// </summary>
		public static void PlayTo(this IFrameAnimable target, int frameNum)
		{
			target.animation.PlayTo(frameNum);
		}
		
		/// <summary>
		/// Returns true if currentFrame == 0
		/// </summary>
		public static bool IsFirstFrame(this IFrameAnimable target)
		{
			return target.currentFrame == 0;
		}
		
		/// <summary>
		/// Returns true if currentFrame == totalFrames - 1
		/// </summary>
		public static bool IsLastFrame(this IFrameAnimable target)
		{
			return target.currentFrame == target.totalFrames - 1;
		}

		/// <summary>
		/// Stops playing and goes to the specified frame.
		/// </summary>
		public static IFrameAnimable GotoAndStop(this IFrameAnimable target, int frameNum)
		{
			target.currentFrame = frameNum;
			target.Stop();
			return target;
		}
		
		/// <summary>
		/// Goes to the specified frame and starts playing forward.
		/// </summary>
		public static IFrameAnimable GotoAndPlay(this IFrameAnimable target, int frameNum)
		{
			target.currentFrame = frameNum;
			target.Play();
			return target;
		}
		
		/// <summary>
		/// Goes to the random frame. Does not stop playing if animation is active.
		/// </summary>
		public static IFrameAnimable GotoRandomFrame(this IFrameAnimable target)
		{
			target.currentFrame = RandomUtil.RandomInt(0, target.totalFrames - 1);
			return target;
		}

		/// <summary>
		/// Goes to the first frame. Does not stop playing if animation is active.
		/// </summary>
		public static IFrameAnimable GotoFirstFrame(this IFrameAnimable target)
		{
			target.currentFrame = 0;
			return target;
		}
		
		/// <summary>
		/// Goes to the last frame. Does not stop playing if animation is active.
		/// </summary>
		public static IFrameAnimable GotoLastFrame(this IFrameAnimable target)
		{
			target.currentFrame = target.totalFrames - 1;
			return target;
		}

		/// <summary>
		/// Goes to the next frame (does nothinf if it was last frame).
		/// </summary>
		public static IFrameAnimable GotoNextFrame(this IFrameAnimable target)
		{
			var currentFrame = target.currentFrame;

			if (currentFrame + 1 < target.totalFrames)
				target.currentFrame = currentFrame + 1;

			return target;
		}
		
		/// <summary>
		/// Goes to the previous frame (does nothinf if it was first frame). 
		/// </summary>
		public static IFrameAnimable GotoPrevFrame(this IFrameAnimable target)
		{
			var currentFrame = target.currentFrame;
			
			if (currentFrame > 0)
				target.currentFrame = currentFrame - 1;

			return target;
		}

		/// <summary>
		/// Goes to the next frame (jumps to the first frame if it was last frame). 
		/// </summary>
		public static IFrameAnimable StepForward(this IFrameAnimable target)
		{
			var currentFrame = target.currentFrame;

			if (currentFrame + 1 < target.totalFrames)
				target.currentFrame = currentFrame + 1;
			else
				target.currentFrame = 0;

			return target;
		}
		
		/// <summary>
		/// Goes to the previous frame (jumps to the last frame if it was first frame). 
		/// </summary>
		public static IFrameAnimable StepBackward(this IFrameAnimable target)
		{
			var currentFrame = target.currentFrame;

			if (currentFrame > 0)
				target.currentFrame = currentFrame - 1;
			else
				target.currentFrame = target.totalFrames - 1;

			return target;
		}
	}
}
