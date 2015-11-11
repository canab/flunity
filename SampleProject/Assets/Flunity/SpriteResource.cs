using System;
using Flunity.Utils;
using UnityEngine;
using Flunity;
using Flunity.Internal;
using System.Collections.Generic;

namespace Flunity
{
	/// <summary>
	/// Holds data exported from flash as BitmapSprite.
	/// </summary>
	public class SpriteResource : ResourceBase, IDisplayResource
	{
		internal const string HD_TAG = "hd:";
		internal const string FRAME_TAG = "f:";

		internal string[] description;

		private SheetFrame[] _frames;
		private bool _isHd;

		public SpriteResource(string path)
			: base(path)
		{
		}

		/// <summary>
		/// Frames data
		/// </summary>
		public SheetFrame[] frames
		{
			get
			{
				EnsureLoaded();
				return _frames;
			}
			protected set { _frames = value; }
		}

		public override void Load()
		{
			if (description == null)
				description = GetDescription();

			frames = GetFrames();
		}

		private string[] GetDescription()
		{
			var contentBundle = bundle as ContentBundle;
			if (contentBundle != null)
				return contentBundle.GetDescription(path);
			
			var filePath = path + ".txt";
			var hdFilePath = path + "$hd.txt";

			if (FlashResources.useHighDefTextures && ResourceHelper.AssetExists(hdFilePath))
				return ResourceHelper.ReadText(hdFilePath).Split('\n');

			if (ResourceHelper.AssetExists(filePath))
				return ResourceHelper.ReadText(filePath).Split('\n');

			return null;
		}

		private TextureInfo GetTexture(string texturePath)
		{
			var result = new TextureInfo();

			var contentBundle = bundle as ContentBundle;
			if (contentBundle != null)
			{
				result.texture = contentBundle.texture;
				result.scale = _isHd ? 0.5f : 1f;
				return result;
			}

			var hdPath = texturePath + "$hd";

			if (FlashResources.useHighDefTextures && ResourceHelper.AssetExists(hdPath))
			{
				result.texture = bundle.LoadContent<Texture2D>(hdPath);
				result.scale = 0.5f;
			}
			else
			{
				result.texture = bundle.LoadContent<Texture2D>(texturePath);
				result.scale = 1;
			}

			if (result.texture == null)
				throw new Exception("Texture not found: " + texturePath);

			return result;
		}

		internal SheetFrame[] GetFrames()
		{
			if (description == null)
			{
				var textureInfo = GetTexture(path);
				return new SheetFrame(textureInfo.texture, Vector2.zero, textureInfo.scale).AsArray();
			}

			var sheetFrames = new List<SheetFrame>(description.Length);

			for (var i = 1; i < description.Length; i++)
			{
				var line = description[i];

				if (line == HD_TAG)
				{
					_isHd = true;
					continue;
				}

				if (line.StartsWith(FRAME_TAG, StringComparison.Ordinal))
				{
					var textureInfo = GetTexture(path);
					var parts = line.Substring(FRAME_TAG.Length).Split(",");

					var frame = new SheetFrame(
						textureInfo.texture,
						new Rect(
							Convert.ToInt32(parts[0]),
							Convert.ToInt32(parts[1]),
							Convert.ToInt32(parts[2]),
							Convert.ToInt32(parts[3])),
						new Vector2(
							Convert.ToInt32(parts[4]),
							Convert.ToInt32(parts[5])),
						textureInfo.scale);

					sheetFrames.Add(frame);
				}
			}

			return sheetFrames.ToArray();
		}
		
		public override void Unload()
		{
			frames = null;
			description = null;
		}

		/// <summary>
		/// Creates sprite instance
		/// </summary>
		public DisplayObject CreateInstance()
		{
			return new FlashSprite(this);
		}

		/// <summary>
		/// Returns size of the 1-st frame
		/// </summary>
		public Vector2 size
		{
			get { return _frames[0].size; }
		}
		
		/// <summary>
		/// Gets the width (determined from 1-st frame)
		/// </summary>
		public float width
		{
			get { return _frames[0].width; }
		}

		/// <summary>
		/// Gets the height (determined from 1-st frame)
		/// </summary>
		public float height
		{
			get { return _frames[0].height; }
		}

		/// <summary>
		/// Returns true if this resource is HD.
		/// Additional scale will be applied when it will be rendered.
		/// </summary>
		public bool isHd
		{
			get { return _isHd; }
		}
	}
}