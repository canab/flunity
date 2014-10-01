using System;
using UnityEngine;
using Flunity.Common;

namespace Flunity.Properties
{
	/// <summary>
	/// Interpolates Vector2 values
	/// </summary>
	public class VectorProperty<TTarget> : ITweenProperty<Vector2> where TTarget : class
	{
		protected readonly Func<TTarget, Vector2> getter;
		protected readonly Action<TTarget, Vector2> setter;

		public VectorProperty(Func<TTarget, Vector2> getter, Action<TTarget, Vector2> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}

		public void WriteValue(float[] array, Vector2 value)
		{
			array[0] = value.x;
			array[1] = value.y;
		}

		public Vector2 ReadValue(float[] array)
		{
			return new Vector2(array[0], array[1]);
		}

		public Vector2 Interpolate(float[] start, float[] end, float t)
		{
			var x = start[0] + t * (end[0] - start[0]);
			var y = start[1] + t * (end[1] - start[1]);

			return new Vector2(x, y);
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