namespace Flunity.Common
{
	/// <summary>
	/// Allows to objects which can be manipulated by somebody specify that
	/// they are in inactive state and all manipulations should be stopped.
	/// 
	/// Example: Tweener stops animate DisplayObject if DisplayObject is detached from stage.
	/// </summary>
	public interface IActiveObject
	{
		bool isActivityEnabled { get; }
	}
}