
namespace Flunity.Common
{
	/// <summary>
	/// Interfacece for tween properties.
	/// </summary>
	public interface ITweenProperty
	{
		void GetValue(float[] array, object target);

		void SetValue(object target, float[] array);
		
		void Interpolate(object target, float[] startValue, float[] endValue, float position);
	}
	
	/// <summary>
	/// Generic interfacece for tween properties of specified type.
	/// </summary>
	public interface ITweenProperty<in TValue> : ITweenProperty
	{
		void WriteValue(float[] array, TValue value);
	}
}