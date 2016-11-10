using System;
using Flunity.UserInterface.Controls;
using UnityEngine;
using Flunity.Utils;

namespace Flunity.UserInterface.Layouts
{
	public class FillLayout : LayoutBase
	{
		public override void apply(ControlBase container)
		{
			var pos = Vector2.zero;

			switch (vAlign)
			{
				case VAlign.TOP:
					pos.y = 0;
					break;
				case VAlign.MIDDLE:
					pos.y = (0.5 * container.height - 0.5 * container.measuredSize.y).RoundToInt();
					break;
				case VAlign.BOTTOM:
					pos.y = (container.height - container.measuredSize.y).RoundToInt();
					break;
			}

			float currentWidth = 0;
			float currentHeight = 0;
			pos.x = 0;
			
			foreach (var item in container)
			{
				item.position = pos;
				
				var itemSize = item.size;
				var dx = currentWidth == 0 ? itemSize.x : itemSize.x + hGap;

				if (currentWidth + dx <= container.measuredSize.x)
				{
					pos.x += dx;
					currentWidth += dx;
					currentHeight = Math.Max(currentHeight, itemSize.y);
				}
				else
				{
					pos.x = 0;
					pos.y += pos.y == 0 ? currentHeight : currentHeight + vGap;
					currentWidth = 0;
					currentHeight = 0;
				}
			}
		}

		public override Vector2 measureSize(ControlBase container)
		{
			float currentWidth = 0;
			float currentHeight = 0;
			float maxWidth = 0;
			float maxHeight = 0;

			foreach (var item in container)
			{
				var itemSize = item.size;
				var dx = currentWidth > 0 ? hGap + itemSize.x : itemSize.x;

				if (currentWidth + itemSize.x <= container.width)
				{
					currentWidth += dx;
					currentHeight = Math.Max(currentHeight, itemSize.y);
				}
				else
				{
					maxWidth = Math.Max(maxWidth, currentWidth);
					maxHeight += maxHeight == 0 ? currentHeight : currentHeight + vGap;
					currentWidth = 0;
					currentHeight = 0;
				}
			}
			
			maxWidth = Math.Max(maxWidth, currentWidth);
			maxHeight += maxHeight == 0 ? currentHeight : currentHeight + vGap;

			return new Vector2(maxWidth, maxHeight);
		}
	}
}