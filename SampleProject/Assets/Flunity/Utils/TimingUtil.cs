using UnityEngine;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class TimingUtil
	{
		public static int TimeToFrames(float time)
		{
			return (int) Mathf.Round(0.001f * time * frameRate);
		}

		public static float FramesToTime(int frames)
		{
			return frames * 1000f / frameRate;
		}

		public static float FramesToTime(long frames)
		{
			return frames * 1000f / frameRate;
		}

		public static long currentTick
		{
			get { return Time.frameCount; }
		}

		public static int frameRate
		{
			get
			{
				var frameRate = Application.targetFrameRate;
				if (frameRate <= 0)
					frameRate = 60;

				return frameRate;
			}
		}
	}
}

