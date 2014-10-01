using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Flunity.Internal
{
	internal class FontManager
	{
		private static readonly List<FontResource> _fonts = new List<FontResource>();
		private static readonly IList<FontResource> _fontsView = new ReadOnlyCollection<FontResource>(_fonts);

		internal static void RegisterFont(FontResource resource)
		{
			_fonts.Add(resource);
		}

		internal static FontResource GetFontResource(string fontName, int fontSize)
		{
			EnsureFontIsRegistered();

			FontResource result = null;

			foreach (var font in _fonts)
			{
				if (result == null)
				{
					result = font;
					continue;
				}

				if (fontName != null && result.fontName != fontName && font.fontName == fontName)
				{
					result = font;
					continue;
				}

				if (result.fontSize < fontSize && font.fontSize > result.fontSize)
				{
					result = font;
					continue;
				}

				if (result.fontSize > fontSize && font.fontSize >= fontSize && font.fontSize < result.fontSize)
					result = font;
			}

			if (result == null)
				result = _fonts[0];

			if (fontName != null && result.fontName != fontName)
				Debug.LogError("Font not found: " + fontName);

			return result;
		}

		public static void EnsureFontIsRegistered()
		{
			if (_fonts.Count == 0)
				throw new Exception("Font resources not found");
		}

		public static IList<FontResource> fonts
		{
			get { return _fontsView; }
		}
	}
}