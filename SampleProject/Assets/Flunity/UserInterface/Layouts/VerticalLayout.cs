using UnityEngine;
using Flunity.Utils;
using Flunity.UserInterface.Controls;

namespace Flunity.UserInterface.Layouts
{
	public class VerticalLayout : LayoutBase
	{
		public override void apply(ControlBase container)
		{
			var pos = Vector2.zero;

			switch (vAlign)
			{
				case VAlign.TOP:
					pos.y = border;
					break;
				case VAlign.MIDDLE:
					pos.y = (0.5 * container.height - 0.5 * container.measuredSize.y).RoundToInt();
					break;
				case VAlign.BOTTOM:
					pos.y = (container.height - container.measuredSize.y - border).RoundToInt();
					break;
			}

			foreach (var item in container)
			{
				switch (hAlign)
				{
					case HAlign.LEFT:
						pos.x = border;
						break;
					case HAlign.CENTER:
						pos.x = (0.5 * container.width - 0.5 * item.width).RoundToInt();
						break;
					case HAlign.RIGHT:
						pos.x = (container.width - item.width - border).RoundToInt();
						break;
				} 

				item.position = pos;
				pos.y += item.height + vGap;
			}
		}

		public override Vector2 measureSize(ControlBase container)
		{
			var s = Vector2.zero;
			foreach (var item in container)
			{
				if (s.y > 0)
					s.y += vGap;
				s.y += item.height;

				if (s.x < item.width)
					s.x = item.width;
			}
			return s + 2 * border * Vector2.one;
		}
	}
}