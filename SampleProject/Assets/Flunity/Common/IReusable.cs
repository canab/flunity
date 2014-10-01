namespace Flunity.Common
{
	/// <summary>
	/// Allows to objects which can be manipulated by somebody specify that
	/// they have been reused (from ObjectPool for instance).
	/// All manipulations with such object should be stopped.
	/// 
	/// Example: Tweener stops animate DisplayObject if object's version has been changed.
	/// </summary>
	public interface IReusable
	{
		/// <summary>
		/// Object's version
		/// </summary>
		uint version { get; }

		/// <summary>
		/// This method increments version and prepares object state for reusing.
		/// </summary>
		void Reuse();
	}
}