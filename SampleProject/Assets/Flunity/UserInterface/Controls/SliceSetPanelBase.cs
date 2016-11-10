using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flunity.UserInterface.Controls
{
	public abstract class SliceSetPanelBase : ControlBase
	{
		protected readonly SpriteResource _resource;
		protected readonly SheetFrame _first;
		protected readonly SheetFrame _middle;
		protected readonly SheetFrame _last;
		protected readonly List<FlashSprite> _sprites = new List<FlashSprite>(8);

		protected int _spritesCount;

		protected SliceSetPanelBase(SpriteResource resource)
		{
			_resource = resource;

			var framesCount = _resource.frames.Length;
			_first = resource.frames[0];
			if (framesCount > 1)
			{
				_middle = _resource.frames[1];
				_last = framesCount > 2 ? _resource.frames[2] : _first;
			}
			
			size = Vector2.zero;
			minSize = size;
		}

		protected override void ApplyLayout()
		{
			RemoveChildren();
			var spritePos = 0;
			for (int i = 0; i < _spritesCount; i++)
			{
				if (i == _sprites.Count)
					_sprites.Add(_resource.CreateInstance() as FlashSprite);

				var sprite = _sprites[i];
				sprite.flipHorizontal = false;
				sprite.flipVertical = false;

				if (i > 0 && i < _spritesCount - 1)
				{
					if (sprite.totalFrames > 1)
						sprite.currentFrame = 1;
				}
				else if (i == _spritesCount - 1)
				{
					if (sprite.totalFrames == 2)
						flipSprite(sprite);
					else if (sprite.totalFrames > 2)
						sprite.currentFrame = 2;
				}

				sprite.parent = this;
				spritePos += (int)setSpritePos(sprite, spritePos);
			}
		}

		public override Vector2 size
		{
			get { return base.size; }
			set
			{
				base.size = _middle == null
					            ? calculateSize1(value)
					            : calculateSize2(value);
			}
		}

		protected abstract float setSpritePos(FlashSprite sprite, int pos);
		protected abstract void flipSprite(FlashSprite sprite);
		protected abstract Vector2 calculateSize1(Vector2 value);
		protected abstract Vector2 calculateSize2(Vector2 value);
	}
}