using UnityEngine;

namespace Flunity.UserInterface.Controls
{
	public interface IButton
	{
		void OnButtonPress();
		
		void OnButtonRelease();
		
		void OnButtonLeave();

		Rect hitArea { get; }
	}
}