using System;
using UnityEngine;
using Flunity.Utils;

namespace Flunity.UserInterface.Controls
{
	public class SliceSetVPanel : SliceSetPanelBase
	{
		public SliceSetVPanel(SpriteResource resource) : base(resource)
		{}

		protected override float setSpritePos(FlashSprite sprite, int pos)
		{
			sprite.y = pos;
			return sprite.height;
		}

		protected override void flipSprite(FlashSprite sprite)
		{
			sprite.flipVertical = true;
		}

		protected override Vector2 calculateSize1(Vector2 value)
		{
			_spritesCount = (int)Math.Max(Math.Ceiling(value.y / _first.height), 1);
			return new Vector2(_first.width,  _spritesCount * _first.height);
		}

		protected override Vector2 calculateSize2(Vector2 value)
		{
			var middleSize = Math.Max(value.y - _first.height - _last.height, 0);
			var middleCount = (int)Math.Max(Math.Round(middleSize / _middle.height), 1);

			_spritesCount = middleCount + 2;

			return new Vector2(MathUtil.Max(_first.width, _middle.width, _last.width),
				_first.height + _last.height + middleCount * _middle.height);
		}
	}
}