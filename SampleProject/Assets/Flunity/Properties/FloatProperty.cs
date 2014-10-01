using System;
using Flunity.Common;

namespace Flunity.Properties
{
	/// <summary>
	/// Interpolates float values
	/// </summary>
	public class FloatProperty<TTarget> : ITweenProperty<float> where TTarget : class
	{
		protected readonly Func<TTarget, float> getter;
		protected readonly Action<TTarget, float> setter;

		public FloatProperty(Func<TTarget, float> getter, Action<TTarget, float> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}

		public void WriteValue(float[] array, float value)
		{
			array[0] = value;
		}

		public float ReadValue(float[] array)
		{
			return array[0];
		}

		public float Interpolate(float[] start, float[] end, float t)
		{
			return start[0] + t * (end[0] - start[0]);
		}

		public void GetValue(float[] array, object target)
		{
			WriteValue(array, getter((TTarget) target));
		}

		public void SetValue(object target, float[] array)
		{
			setter((TTarget) target, ReadValue(array));
		}

		public void Interpolate(object target, float[] startValue, float[] endValue, float position)
		{
			setter((TTarget) target, Interpolate(startValue, endValue, position));
		}
	}
}
