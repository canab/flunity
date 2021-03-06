﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Flunity.Internal;

namespace Flunity
{
	internal class CharInfo
	{
		public char symbol;
		public int frameNum;
		public float symbolWidth;
		public Vector2 offset;
	}
	
	/// <summary>
	/// Represent font symbols data exported from flash.
	/// </summary>
	public class FontResource : SpriteResource
	{
		#region _replacementChars

		private static readonly char[] _replacementChars = 
		{
			'і', 'i',
			'І', 'I',
			'ї', 'i',
			'Ї', 'I',
			'є', 'e',
			'Є', 'E',
			'¡', 'i',
			'´', '\'',
			'à', 'a',
			'â', 'a',
			'ã', 'a',
			'ä', 'a',
			'å', 'a',
			'ç', 'c',
			'è', 'e',
			'é', 'e',
			'ê', 'e',
			'ë', 'e',
			'ì', 'i',
			'í', 'i',
			'î', 'i',
			'ï', 'i',
			'ñ', 'n',
			'ò', 'o',
			'ó', 'o',
			'ô', 'o',
			'õ', 'o',
			'ö', 'o',
			'×', 'x',
			'ø', 'o',
			'ù', 'u',
			'ú', 'u',
			'û', 'u',
			'ü', 'u',
			'ý', 'y',
			'ÿ', 'y',
			'À', 'A',
			'Â', 'A',
			'Ã', 'A',
			'Ä', 'A',
			'Å', 'A',
			'Ç', 'C',
			'È', 'E',
			'É', 'E',
			'Ê', 'E',
			'Ë', 'E',
			'Ì', 'I',
			'Í', 'I',
			'Î', 'I',
			'Ï', 'I',
			'Ñ', 'N',
			'Ò', 'O',
			'Ó', 'O',
			'Ô', 'O',
			'Õ', 'O',
			'Ö', 'O',
			'×', 'X',
			'Ø', 'O',
			'Ù', 'U',
			'Ú', 'U',
			'Û', 'U',
			'Ü', 'U',
			'Ý', 'Y',
			'Ÿ', 'Y',
			'К', 'K',
			'Е', 'E',
			'Н', 'H',
			'З', '3',
			'Х', 'X',
			'В', 'B',
			'А', 'A',
			'Р', 'P',
			'О', 'O',
			'С', 'C',
			'М', 'M',
			'Т', 'T',
			'у', 'y',
			'е', 'e',
			'х', 'x',
			'а', 'a',
			'р', 'p',
			'о', 'o',
			'с', 'c',
		};

		#endregion
		
		private Dictionary<char, CharInfo> _charmap = new Dictionary<char, CharInfo>();

		public string fontName { get; private set; }
		public int fontSize { get; private set; }
		public int letterSpacing { get; private set; }
		public int rowHeight { get; private set; }
		public Vector2 offset { get; private set; }
		
		public FontResource(string path) : base(path)
		{
			FontManager.RegisterFont(this);
		}

		public override void Load()
		{
			base.Load();

			_charmap = ReadCharMap();

			const string FONT_TAG = "font:";
			var format = description.First(it => it.StartsWith (FONT_TAG, StringComparison.Ordinal));
			var formatParts = format.Substring(FONT_TAG.Length).Split(',');
			
			fontName = formatParts[0];
			fontSize = Convert.ToInt32(formatParts[1]);
			offset = new Vector2(Convert.ToInt32(formatParts[2]), Convert.ToInt32(formatParts[3]));
			rowHeight = Convert.ToInt32(formatParts[4]);
			letterSpacing = Convert.ToInt32(formatParts[5]);
		}

		private Dictionary<char, CharInfo> ReadCharMap() 
		{
			var result = new Dictionary<char, CharInfo>();
			var textureScale = isHd ? 0.5f : 1f;
			var frameNum = 0;
			
			for (int i = 0; i < description.Length; i++)
			{
				var line = description[i];
				if (!line.StartsWith(FRAME_TAG, StringComparison.Ordinal))
					continue;

				var parts = line.Substring(FRAME_TAG.Length).Split(',');

				var charInfo = new CharInfo
				{
					frameNum = frameNum,
					symbol = line[line.Length - 1],
					symbolWidth = Convert.ToInt32(parts[6]) * textureScale,
					offset = new Vector2
					{
						x = Convert.ToInt32(parts[7]) * textureScale,
						y = Convert.ToInt32(parts[8]) * textureScale,
					}
				};

				frameNum += 1;

				result[charInfo.symbol] = charInfo;
			}
			return result;
		}

		internal CharInfo GetCharInfo(char c)
		{
			CharInfo info;
			if (!_charmap.TryGetValue(c, out info))
			{
				var index = Array.IndexOf(_replacementChars, c);
				if (index >= 0 && index % 2 == 0)
					_charmap.TryGetValue(_replacementChars[index + 1], out info);
			}
			
			return info;
		}
		
		internal CharInfo GetDefaultCharInfo()
		{
			return GetCharInfo(' ');
		}

		public override void Unload()
		{
			base.Unload();
			_charmap = null;
		}
	}
}