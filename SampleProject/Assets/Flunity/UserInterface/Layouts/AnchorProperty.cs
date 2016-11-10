using System;

namespace Flunity.UserInterface.Layouts
{
	public class AnchorProperty
	{
		public readonly Func<DisplayObject, float> getter;
		public readonly Action<DisplayObject, float> setter;

		public AnchorProperty(Func<DisplayObject, float> getter, Action<DisplayObject, float> setter)
		{
			this.getter = getter;
			this.setter = setter;
		}
	}
}