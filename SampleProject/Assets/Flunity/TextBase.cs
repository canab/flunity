using System;
using Flunity.Utils;
using UnityEngine;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Base class for text rendering. See TextField, TextLabel
	/// </summary>
	public abstract class TextBase : DisplayObject
	{
		public HAlign hAlignment { get; set; }
		public VAlign vAlignment { get; set; }

		protected readonly FontResource resource;

		public Vector2 shadowOffset = Vector2.zero;
		public Color textColor = Color.white;
		public Color shadowColor = Color.white;

		private string _text = "";
		private Vector2 _textSize;
		private bool _textSizeValid = true;

		private readonly QuadCollection _textQuads = new QuadCollection();
		private readonly QuadCollection _shadowQuads = new QuadCollection();

		protected Vector2 _size = Vector2.zero;
		private bool _autoSize = false;
		private float _fontScale = 1;

		protected TextBase(FontResource resource, float fontScale)
		{
			this.resource = resource;
			this.fontScale = fontScale;
		}

		protected TextBase(string fontName, int fontSize)
		{
			resource = FontManager.GetFontResource(fontName, fontSize);
			fontScale = fontSize > 0
				? (float)fontSize / resource.fontSize
				: 1;
		}

		protected abstract Vector2 GetTextSize();
		
		private void InvalidateTextSize()
		{
			_textSizeValid = false;
		}

		private void ValidateTextSize()
		{
			_textSize = GetTextSize();
			_textSizeValid = true;
		}
		
		public override Rect GetInternalBounds()
		{
			return new Rect(0, 0, _size.x, _size.y);
		}

		public virtual string text
		{
			get { return _text; }
			set
			{
				if (_text != value)
				{
					_text = value;
					transformDirty = true;
					InvalidateTextSize();
				}
			}
		}

		/// <summary>
		/// Scale of the font
		/// </summary>
		public float fontScale
		{
			get { return _fontScale; }
			set
			{
				_fontScale = value;
				transformDirty = false;
				InvalidateTextSize();
			}
		}

		/// <summary>
		/// Size of the text symbols
		/// </summary>
		public Vector2 textSize
		{
			get
			{
				if (!_textSizeValid)
					ValidateTextSize();

				return _textSize;
			}
		}

		/// <summary>
		/// returns textSize.x
		/// </summary>
		public float textWidth
		{
			get { return textSize.x; }
		}

		/// <summary>
		/// returns textSize.y
		/// </summary>
		public float textHeight
		{
			get { return textSize.y; }
		}

		/// <summary>
		/// Calculates TopLeft position of the first symbol (depending on alignment)
		/// </summary>
		public Vector2 GetTextTopLeft()
		{
			if (_autoSize)
				return resource.offset;
			
			var result = Vector2.zero;
			var fontOffset = resource.offset;

			switch (hAlignment)
			{
				case HAlign.LEFT:
					result.x = fontOffset.x;
					break;
				case HAlign.CENTER:
					result.x = (0.5f * (width - textSize.x)).RoundToInt();
					break;
				case HAlign.RIGHT:
					result.x = (width - textSize.x) - fontOffset.x;
					break;
			}

			switch (vAlignment)
			{
				case VAlign.TOP:
					result.y = fontOffset.y;
					break;
				case VAlign.MIDDLE:
					result.y = (0.5f * (height - textSize.y)).RoundToInt();
					break;
				case VAlign.BOTTOM:
					result.y = (height - textSize.y) - fontOffset.y;
					break;
			}

			return result;
		}

		/// <summary>
		/// Calculates bounds of the text sybmols
		/// </summary>
		public Rect GetTextBounds()
		{
			var localPos = GetTextTopLeft();
			var localSize = textSize;

			return new Rect(
				(int)Math.Floor(localPos.x),
				(int)Math.Floor(localPos.y),
				(int)Math.Ceiling(localPos.x + localSize.x),
				(int)Math.Ceiling(localPos.y + localSize.y));
		}

		protected Vector2 CalculateTextSize(string value)
		{
			var charsWidth = 0f;
			var spacing = (value.Length - 1) * resource.letterSpacing;

			foreach (var c in value)
			{
				if (c < ' ')
					continue;
				var charInfo = resource.GetCharInfo(c) ?? resource.GetDefaultCharInfo();
				charsWidth += charInfo.symbolWidth;
			}

			var w = (charsWidth + spacing) * fontScale;
			var h = resource.rowHeight * fontScale;
			return new Vector2(w, h).RoundToInt();
		}

		protected void AddWordQuads(string word, Vector2 position)
		{
			AddQuads(_textQuads, word, position, textColor);
			
			if (shadowOffset != Vector2.zero)
				AddQuads(_shadowQuads, word, position + shadowOffset, shadowColor);
		}

		protected void ClearQuads()
		{
			_textQuads.Clear();
			_shadowQuads.Clear();
		}

		private void AddQuads(QuadCollection quads, string word, Vector2 positionOffset, Color quadColor)
		{
			var charPos = positionOffset;

			foreach (var c in word)
			{
				var charInfo = resource.GetCharInfo(c) ?? resource.GetDefaultCharInfo();
				var frame = resource.frames[charInfo.frameNum];

				var anchorPoint = new Vector2(
					anchor.x + frame.textureAnchor.x,
					anchor.y + frame.textureAnchor.y);

				var framePos = new Vector2(
					charPos.x + charInfo.offset.x * fontScale,
					charPos.y + charInfo.offset.y * fontScale);

				charPos.x += (charInfo.symbolWidth + resource.letterSpacing) * fontScale;

				var drawColor = new ColorTransform(quadColor);
				ColorTransform.Compose(ref drawColor, ref compositeColor, out drawColor);

				AddQuad(quads, frame, ref framePos, ref anchorPoint, ref drawColor);
			}
		}

		public override void Draw()
		{
			var texture = resource.frames[0].texture;

			if (DebugDraw.drawTextField)
				DebugDraw.DrawRect(this, GetInternalBounds(), DebugDraw.drawTextFieldColor);

			stage.sceneBatch.DrawQuads(texture, _shadowQuads);
			stage.sceneBatch.DrawQuads(texture, _textQuads);
		}

		internal protected override void UpdateColor()
		{
			base.UpdateColor();

			var textTransform = new ColorTransform(textColor);
			ColorTransform.Compose(ref textTransform, ref compositeColor, out textTransform);
			_textQuads.UpdateColor(ref textTransform);

			if (shadowOffset != Vector2.zero)
			{
				var shadTransform = new ColorTransform(shadowColor);
				ColorTransform.Compose(ref shadTransform, ref compositeColor, out shadTransform);
				_shadowQuads.UpdateColor(ref shadTransform);
			}
		}

		protected void AddQuad(QuadCollection quads, SheetFrame frame,
			ref Vector2 charPos, ref Vector2 anchor, ref ColorTransform colorMult)
		{
			var texture = frame.texture;
			var textureBounds = frame.textureBounds;

			Matrix4x4 localMatrix, globalMatrix;
			var letterScale = fontScale * frame.textureScale;
			var letterScaleVec = new Vector2(letterScale, letterScale);
			MatrixUtil.Create2D(ref letterScaleVec, 0, ref charPos, out localMatrix);
			MatrixUtil.Multiply(ref localMatrix, ref compositeMatrix, out globalMatrix);

			var textureSize = new Vector2(texture.width, texture.height);

			var spriteQuad = new SpriteQuad();
			spriteQuad.UpdateTransform(ref globalMatrix, ref textureBounds, ref textureSize, ref anchor);
			spriteQuad.UpdateColor(ref colorMult);

			quads.Add(spriteQuad);
		}

		public override Vector2 size
		{
			get { return _size; }
			set { _size = value; }
		}

		/// <summary>
		/// Size will be set to the actual text size.
		/// </summary>
		/// <value><c>true</c> if auto size; otherwise, <c>false</c>.</value>
		public bool autoSize
		{
			get { return _autoSize; }
			set { _autoSize = value; }
		}
	}
}