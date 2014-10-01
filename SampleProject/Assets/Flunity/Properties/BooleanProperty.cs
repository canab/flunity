using System;
using Flunity.Common;

namespace Flunity.Properties
{
	/// <summary>
	/// Interpolates boolean values
	/// </summary>
	public class BooleanProperty<TTarget> : ITweenProperty<bool> where TTarget : class
	{
		protected readonly Func<TTarget, bool> getter;
		protected readonly Action<TTarget, bool> setter;

		public BooleanProperty(Func<TTarget, bool> getter, Action<TTarget, bool> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}

		public void WriteValue(float[] array, bool value)
		{
			array[0] = value ? 0 : 1;
		}

		public bool ReadValue(float[] array)
		{
			return array[0] < 0.5f;
		}

		public bool Interpolate(float[] start, float[] end, float t)
		{
			return start[0] > 0.5f;
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