using UnityEngine;
using Flunity.UserInterface.Controls;
using Flunity.Utils;

namespace Flunity.UserInterface.Layouts
{
	public class HorizontalLayout : LayoutBase
	{
		public override void apply(ControlBase container)
		{
			var pos = Vector2.zero;

			switch (hAlign)
			{
				case HAlign.LEFT:
					pos.x = border;
					break;
				case HAlign.CENTER:
					pos.x = (0.5 * container.width - 0.5 * container.measuredSize.x).RoundToInt();
					break;
				case HAlign.RIGHT:
					pos.x = (container.width - container.measuredSize.x - border).RoundToInt();
					break;
			}

			foreach (var item in container)
			{
				switch (vAlign)
				{
					case VAlign.TOP:
						pos.y = border;
						break;
					case VAlign.MIDDLE:
						pos.y = (0.5 * container.height - 0.5 * item.height).RoundToInt();
						break;
					case VAlign.BOTTOM:
						pos.y = (container.height - item.height - border).RoundToInt();
						break;
				} 
				
				item.position = pos;
				pos.x += item.width + hGap;
			}
		}

		public override Vector2 measureSize(ControlBase container)
		{
			var s = Vector2.zero;
			foreach (var item in container)
			{
				if (s.x > 0)
					s.x += hGap;
				s.x += item.width;

				if (s.y < item.height)
					s.y = item.height;
			}
			return s + 2 * border * Vector2.one;
		}
	}
}