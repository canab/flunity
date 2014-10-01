using System;
using Flunity.Utils;
using Flunity.Common;

namespace Flunity.Properties
{
	/// <summary>
	/// Interpolates int values
	/// </summary>
	public class IntProperty<TTarget> : ITweenProperty<int> where TTarget : class
	{
		protected readonly Func<TTarget, int> getter;
		protected readonly Action<TTarget, int> setter;

		public IntProperty(Func<TTarget, int> getter, Action<TTarget, int> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}

		public void WriteValue(float[] array, int value)
		{
			array[0] = value;
		}

		public int ReadValue(float[] array)
		{
			return array[0].RoundToInt();
		}

		public int Interpolate(float[] start, float[] end, float t)
		{
			return (start[0] + t * (end[0] - start[0])).RoundToInt();
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