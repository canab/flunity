using System;
using Flunity.UserInterface.Layouts;

namespace Flunity.UserInterface.Controls
{
	public class ContainerBase : ControlBase
	{
		private LayoutBase _layout;

		protected ContainerBase(LayoutBase layout)
		{
			base.layout = _layout = layout;
		}

		public ContainerBase(DisplayContainer parent, LayoutBase layout)
			: this(layout)
		{
			this.parent = parent;
		}

		public int hGap
		{
			get { return _layout.hGap; }
			set { _layout.hGap = value; }
		}
		
		public int vGap
		{
			get { return _layout.vGap; }
			set { _layout.vGap = value; }
		}

		public HAlign hAlign
		{
			get { return _layout.hAlign; }
			set { _layout.hAlign = value; }
		}

		public VAlign vAlign
		{
			get { return _layout.vAlign; }
			set { _layout.vAlign = value; }
		}
		
		public int border
		{
			get { return _layout.border; }
			set { _layout.border = value; }
		}

		public override ILayout layout
		{
			get { return _layout; }
			set { throw new NotSupportedException();}
		}
	}
}