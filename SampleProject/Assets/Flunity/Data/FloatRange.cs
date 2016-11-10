using System;
using UnityEngine;

namespace Flunity.Common.Data
{
	public struct FloatRange
	{
		public float min;
		public float max;

		public FloatRange(float min, float max)
		{
			this.min = min;
			this.max = max;
		}

		public float claim(float value)
		{
			var result = 0.0f;
			Mathf.Clamp(result, min, max);
			return result;
		}

		public bool contains(float value)
		{
			return value >= min && value <= max;
		}

		public float interpolate(float value)
		{
			if (max == min)
				return min;
			
			return min + value * (max - min);
		}

		public float interpolateInverse(float value)
		{
			if (max == min)
				return min;
			
			return max - value * (max - min);
		}

		public float getRelative(float value)
		{
			if (max == min)
				return 0;
			
			return (value - min) / (max - min);
		}
		
		public float getRelativeInverse(float value)
		{
			if (max == min)
				return 0;

			return (max - value) / (max - min);
		}

		public override string ToString()
		{
			return string.Format("Range[{0}..{1}]", min, max);
		}

		public float length
		{
			get { return max - min; }
		}
	}
}
