using System;
using UnityEngine;
using Flunity.Utils;

namespace Flunity.UserInterface.Controls
{
	public class SliceSetHPanel : SliceSetPanelBase
	{
		public SliceSetHPanel(SpriteResource resource) : base(resource)
		{}

		protected override float setSpritePos(FlashSprite sprite, int pos)
		{
			sprite.x = pos;
			return sprite.width;
		}

		protected override void flipSprite(FlashSprite sprite)
		{
			sprite.flipHorizontal = true;
		}

		protected override Vector2 calculateSize1(Vector2 value)
		{
			_spritesCount = (int)Math.Max(Math.Ceiling(value.x / _first.width), 1);
			return new Vector2(_spritesCount * _first.width, _first.height);
		}

		protected override Vector2 calculateSize2(Vector2 value)
		{
			var middleSize = Math.Max(value.x - _first.width - _last.width, 0);
			var middleCount = (int)Math.Max(Math.Round(middleSize / _middle.width), 1);

			_spritesCount = middleCount + 2;

			return new Vector2(_first.width + _last.width + middleCount * _middle.width,
			                   MathUtil.Max(_first.height, _middle.height, _last.height));
		}
	}
}