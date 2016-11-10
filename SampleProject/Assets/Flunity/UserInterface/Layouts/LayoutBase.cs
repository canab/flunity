using UnityEngine;
using Flunity.UserInterface.Controls;

namespace Flunity.UserInterface.Layouts
{
	public abstract class LayoutBase : ILayout
	{
		private int _border = 0;
		private int _hGap = 0;
		private int _vGap = 0;
		private HAlign _hAlign = HAlign.LEFT;
		private VAlign _vAlign = VAlign.TOP;

		public abstract void apply(ControlBase container);
		public abstract Vector2 measureSize(ControlBase container);

		public int hGap
		{
			get { return _hGap; }
			set { _hGap = value; }
		}
		
		public int vGap
		{
			get { return _vGap; }
			set { _vGap = value; }
		}

		public HAlign hAlign
		{
			get { return _hAlign; }
			set { _hAlign = value; }
		}

		public VAlign vAlign
		{
			get { return _vAlign; }
			set { _vAlign = value; }
		}
		
		public int border
		{
			get { return _border; }
			set { _border = value; }
		}

	}
}