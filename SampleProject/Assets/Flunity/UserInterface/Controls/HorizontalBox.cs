using Flunity.UserInterface.Layouts;

namespace Flunity.UserInterface.Controls
{
	public class HorizontalBox : ContainerBase
	{
		public HorizontalBox() : base(new HorizontalLayout())
		{
			autoSize = true;
		}

		public HorizontalBox(DisplayContainer parent) : this()
		{
			this.parent = parent;
		}
	}
}