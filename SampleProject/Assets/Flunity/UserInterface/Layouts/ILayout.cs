using UnityEngine;
using Flunity.UserInterface.Controls;

namespace Flunity.UserInterface.Layouts
{
	public interface ILayout
	{
		void apply(ControlBase container);
		Vector2 measureSize(ControlBase container);
	}
}