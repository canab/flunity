using Flunity.UserInterface.Layouts;
namespace Flunity.UserInterface.Controls
{
	public class VerticalBox : ContainerBase
	{
		public VerticalBox() : base(new VerticalLayout())
		{
			autoSize = true;
		}

		public VerticalBox(DisplayContainer parent) : this()
		{
			this.parent = parent;
		}
	}
}