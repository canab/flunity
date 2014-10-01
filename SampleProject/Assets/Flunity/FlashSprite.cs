using System;
using UnityEngine;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Bitmap sprite. Has one or more bitmap frames.
	/// </summary>
	public class FlashSprite : DisplayObject
	{
		private SpriteResource _resource;
		private SheetFrame _frame;
		private Rect? _scrollRect;
		private Rect _textureScrollRect;
		private SpriteQuad _quad = new SpriteQuad();
		private bool _flipHorizontal;
		private bool _flipVertical;

		public FlashSprite(DisplayContainer parent, SpriteResource resource)
		{
			this.resource = resource;
			this.parent = parent;
		}

		public FlashSprite(SpriteResource resource)
		{
			this.resource = resource;
		}

		public FlashSprite(Texture2D texture)
		{
			_frame = new SheetFrame(texture);
		}

		protected override void ResetDisplayObject()
		{
			_frame = null;
			_scrollRect = null;
			_flipHorizontal = false;
			_flipVertical = false;
			base.ResetDisplayObject();
		}

		protected override void OnFrameChange()
		{
			_frame = _resource.frames[currentFrame];
			
			transformDirty = true;
			colorDirty = true;
		}

		#region draw

		internal protected override void UpdateTransform()
		{
			base.UpdateTransform();

			Rect textureRect;

			if (_scrollRect.HasValue)
			{
				var sheetRect = new Rect(
					                _textureScrollRect.x + _frame.textureBounds.x + _frame.textureAnchor.x,
					                _textureScrollRect.y + _frame.textureBounds.y + _frame.textureAnchor.y,
					                _textureScrollRect.width,
					                _textureScrollRect.height);

				textureRect = sheetRect.GetIntersection(_frame.textureBounds);
			}
			else
			{
				textureRect = _frame.textureBounds;
			}

			var anchorPoint = new Vector2(
				                  anchor.x / _frame.textureScale + _frame.textureAnchor.x,
				                  anchor.y / _frame.textureScale + _frame.textureAnchor.y);

			var flip = TextureFlip.NONE;
			if (_flipHorizontal)
				flip |= TextureFlip.HORIZONTAL;
			if (_flipVertical)
				flip |= TextureFlip.VERTICAL;

			var texture = _frame.texture;
			var textureScale = _frame.textureScale;
			var textureSize = new Vector2(texture.width, texture.height);
			var globalMatrix = MatrixUtil.CreateScale2D(textureScale) * compositeMatrix;

			_quad.UpdateTransform(ref globalMatrix, ref textureRect, ref textureSize, ref anchorPoint, flip);
		}

		internal protected override void UpdateColor()
		{
			base.UpdateColor();
			_quad.UpdateColor(ref compositeColor);
		}

		public override void Draw()
		{
			stage.sceneBatch.DrawQuad(_frame.texture, ref _quad);
		}

		#endregion

		#region properties

		public override Vector2 size
		{
			get
			{
				return new Vector2(
					Math.Abs(_frame.width * scale.x),
					Math.Abs(_frame.height * scale.y));
			}
			set
			{
				var signX = scaleX < 0 ? -1 : 1;
				var signY = scaleY < 0 ? -1 : 1;

				// Analysis disable CompareOfFloatsByEqualityOperator
				scale = new Vector2(
					(_frame.width == 0) ? signX : signX * value.x / _frame.width,
					(_frame.height == 0) ? signY : signY * value.y / _frame.height);
				// Analysis restore CompareOfFloatsByEqualityOperator
			}
		}

		public override Rect GetInternalBounds()
		{
			var a = anchor;
			var f = _frame.bounds;
			return new Rect(f.x - a.x, f.y - a.y, f.width, f.height);
		}

		public Rect? scrollRect
		{
			get { return _scrollRect; }
			set
			{
				_scrollRect = value;

				if (_scrollRect != null)
				{
					_textureScrollRect = new Rect(
						_scrollRect.Value.x / _frame.textureScale,
						_scrollRect.Value.y / _frame.textureScale,
						_scrollRect.Value.width / _frame.textureScale,
						_scrollRect.Value.height / _frame.textureScale);
				}
			}
		}

		public SpriteResource resource
		{
			get { return _resource; }
			set
			{
				_resource = value;

				totalFrames = _resource.frames.Length;

				if (currentFrame != 0)
					currentFrame = 0;
				else
					OnFrameChange();
			}
		}

		public bool flipHorizontal
		{
			get { return _flipHorizontal; }
			set { _flipHorizontal = value; }
		}

		public bool flipVertical
		{
			get { return _flipVertical; }
			set { _flipVertical = value; }
		}

		public SheetFrame sheetFrame
		{
			get { return _frame; }
		}

		#endregion

	}
}