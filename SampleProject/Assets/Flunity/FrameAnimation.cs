using System;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Holds animation data for containing DisplayObject.
	/// Updates DisplayObject's currentFrame when animation is active.
	/// </summary>
	public class FrameAnimation
	{
		public static int defaultTicksPerFrame = 1;

		public Action completeHandler;
		public int ticksPerFrame = defaultTicksPerFrame;
		public bool isActive;

		private readonly IFrameAnimable _target;
		private int _tickCounter;
		private int _endFrame;
		private bool _looping;
		private int _step;
		private int _totalFrames;
		
		public FrameAnimation(IFrameAnimable target)
		{
			_target = target;
		}

		public void PlayTo(int endFrame)
		{
			_totalFrames = _target.totalFrames;
			_looping = false;
			_endFrame = endFrame.ClampInt(0, _target.totalFrames - 1);
			_step = _endFrame > _target.currentFrame ? 1 : -1;

			isActive = true;
		}

		public void PlayLoop(int step)
		{
			_totalFrames = _target.totalFrames;
			_looping = true;
			_step = step;

			isActive = true;
		}

		public void Stop()
		{
			isActive = false;
		}
		
		internal void DoStep()
		{
			if (++_tickCounter < ticksPerFrame)
				return;

			_tickCounter = 0;
			
			var currentFrame = _target.currentFrame;
			
			if (!_looping && currentFrame == _endFrame)
			{
				Stop();
				completeHandler.Dispatch();
				return;
			}

			var nextFrame = currentFrame + _step;
			if (nextFrame < 0)
				nextFrame = _totalFrames - 1;
			else if (nextFrame >= _totalFrames)
				nextFrame = 0;

			_target.currentFrame = nextFrame;
		}
	}
}
