using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// DisplayObject to render single-line text
	/// </summary>
	public class TextLabel : TextBase
	{
		private int _maxLength = 0;
		private string _maxLengthSuffix = "...";

		public TextLabel(DisplayContainer parent, string fontName, int fontSize = 0) : this(fontName, fontSize)
		{
			this.parent = parent;
		}
		
		public TextLabel(string fontName, int fontSize = 0) : base(fontName, fontSize)
		{
		}

		public TextLabel(DisplayContainer parent, int fontSize = 0)
			: this(fontSize)
		{
			this.parent = parent;
		}

		public TextLabel(int fontSize = 0) : base(null, fontSize)
		{
		}

		internal protected override void UpdateTransform()
		{
			base.UpdateTransform();
			ClearQuads();
			AddWordQuads(text, GetTextTopLeft());
		}

		public override string text
		{
			get { return base.text; }
			set
			{
				base.text = (_maxLength != 0)
					? TryFitText(value)
					: value;
			}
		}

		private string TryFitText(string textToFit)
		{
			if (CalculateTextSize(textToFit).x > _maxLength)
			{
				while (CalculateTextSize(textToFit + _maxLengthSuffix).x > _maxLength && textToFit.Length > 0)
				{
					textToFit = textToFit.Remove(textToFit.Length - 1);
				}
				textToFit = textToFit + _maxLengthSuffix;
			}
			return textToFit;
		}

		protected override Vector2 GetTextSize()
		{
			return text != null
				? CalculateTextSize(text)
				: Vector2.zero;
		}

		/// <summary>
		/// If test doesn't fit, it will be trancated.
		/// </summary>
		public int maxLength
		{
			get { return _maxLength; }
			set { _maxLength = value; }
		}

		/// <summary>
		/// Chars will be added to truncated text.
		/// Default is "..."
		/// </summary>
		public string maxLengthSuffix
		{
			get { return _maxLengthSuffix; }
			set { _maxLengthSuffix = value; }
		}

		public override Vector2 size 
		{
			get
			{
				return autoSize ? textSize : _size;
			}
			set 
			{
				_size = value;
			}
		}
	}
}