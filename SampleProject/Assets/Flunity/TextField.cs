using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flunity.Utils;

namespace Flunity
{
	/// <summary>
	/// DisplayObject to render multiline text
	/// </summary>
	public class TextField : TextBase
	{
		private const char SPACE_CHAR = ' ';

		public float rowSpacing = 0;

		private readonly float _rowHeight;
		private readonly List<TextRow> _rows = new List<TextRow>(4);
		private bool _textDirty = true;

		public TextField(DisplayContainer parent, string fontName, int fontSize = 0) : this(fontName, fontSize)
		{
			this.parent = parent;
		}

		public TextField(string fontName, int fontSize = 0) : base(fontName, fontSize)
		{
			_rowHeight = resource.rowHeight * fontScale;
		}

		public TextField(DisplayContainer parent, int fontSize = 0)
			: this(fontSize)
		{
			this.parent = parent;
		}

		public TextField(int fontSize = 0) : base(null, fontSize)
		{
			_rowHeight = resource.rowHeight * fontScale;
		}

		internal protected override void UpdateTransform()
		{
			base.UpdateTransform();
			
			ValidateText();
			ClearQuads();
			
			var textTopLeft = GetTextTopLeft();
			var rowPos = textTopLeft;

			foreach (var row in _rows)
			{
				rowPos.x = GetRowOffsetX(row.textWidth);
				AddWordQuads(row.text, rowPos);
				rowPos.y += ((_rowHeight + rowSpacing));
			}
		}

		private float GetRowOffsetX(float rowWidth)
		{
			if (hAlignment == HAlign.LEFT)
				return 0;

			if (hAlignment == HAlign.CENTER)
				return (0.5f * (width - rowWidth)).RoundToInt();

			return width - rowWidth;
		}

		protected override Vector2 GetTextSize()
		{
			ValidateText();

			var w = _rows.Max(r => r.textWidth);
			var h = _rows.Count * _rowHeight + (_rows.Count - 1) * rowSpacing;

			return new Vector2(
				(float) Math.Ceiling(w),
				(float) Math.Ceiling(h));
		}

		internal void ValidateText()
		{
			if (_textDirty)
			{
				CreateRows();
				_textDirty = false;
			}
		}

		private void CreateRows()
		{
			var spaceCharInfo = resource.GetCharInfo(SPACE_CHAR);
			var spaceWidth = spaceCharInfo.symbolWidth * fontScale;

			_rows.Clear();
			
			var textString = text ?? "";

			var words = string.Join("\n ", textString.Split('\n')).Split(SPACE_CHAR);
			var currentRow = TextRow.empty;
			var wordIndex = 0;

			// don't use size/width properties to avoid stack overflow when autoSize == true;
			var maxWidth = _size.x;

			while (wordIndex < words.Length)
			{
				var word = words[wordIndex];
				if (word == null)
					continue;

				var explicitLineBrake = (word.Length > 0 && word[word.Length - 1] == '\n');
				if (explicitLineBrake)
					word = word.Replace("\n", "");

                if (word.Length <= 0)
                {
	                if (wordIndex == words.Length - 1 || explicitLineBrake)
	                {
						_rows.Add(currentRow);
						currentRow = TextRow.empty;
	                }

					wordIndex++;
                    continue;
                }

				var wordWidth = CalculateTextSize(word).x;
				var spacing = currentRow.text.Length == 0 ? 0 : spaceWidth;
				var doesFit = currentRow.textWidth + spacing + wordWidth <= maxWidth;
				var needNewLine = !doesFit || wordIndex == words.Length - 1 || explicitLineBrake;

				if (currentRow.text.Length == 0)
				{
					currentRow.text += word;
					currentRow.textWidth += wordWidth;
					wordIndex++;
				}
				else if (doesFit)
				{
					currentRow.text += SPACE_CHAR + word;
					currentRow.textWidth += spaceWidth + wordWidth;
					wordIndex++;
				}

				if (needNewLine)
				{
					_rows.Add(currentRow);
					currentRow = TextRow.empty;
				}
			}
		}

		/// <summary>
		/// Rows count
		/// </summary>
		public int rowsCount
		{
			get { return _rows.Count; }
		}

		public override string text
		{
			get { return base.text; }
			set
			{
				base.text = value;
				_textDirty = true;
			}
		}

		public override Vector2 size 
		{
			get
			{
				return autoSize
					? new Vector2(_size.x, textSize.y)
					: _size;
			}
			set 
			{
				_size = value;
				_textDirty = true;
			}
		}
	}

	internal struct TextRow
	{
		public static TextRow empty
		{
			get { return new TextRow {text = "", textWidth = 0}; }
		}

		public String text;
		public float textWidth;
	}
}