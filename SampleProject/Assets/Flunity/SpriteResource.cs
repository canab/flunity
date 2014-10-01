using System;
using System.Linq;
using System.Xml.Linq;
using Flunity.Utils;
using UnityEngine;
using Flunity;
using Flunity.Internal;
using Flunity.Common;

namespace Flunity
{
	/// <summary>
	/// Holds data exported from flash as BitmapSprite.
	/// </summary>
	public class SpriteResource : ResourceBase, IDisplayResource
	{
		internal XElement description;

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
			{
				description = GetDescription();

				_isHd = description != null
					&& description.Attribute("hd") != null
					&& description.Attribute("hd").Value == "true";
			}

			frames = GetFrames();
		}

		private XElement GetDescription()
		{
			var contentBundle = bundle as ContentBundle;
			if (contentBundle != null)
				return contentBundle.GetDescription(path);
			
			var filePath = FlashResources.GetFullPath(path + ".xml");
			var hdFilePath = FlashResources.GetFullPath(path + "$hd.xml");

			if (FlashResources.useHighDefTextures && ResourceHelper.AssetExists(hdFilePath))
				return ResourceHelper.ReadXml(hdFilePath).Root;

			if (ResourceHelper.AssetExists(filePath))
				return ResourceHelper.ReadXml(filePath).Root;

			return null;
		}

		private TextureInfo GetTexture(string texturePath)
		{
			var contentBundle = bundle as ContentBundle;
			if (contentBundle != null)
				return contentBundle.GetTexture(texturePath);
			
			var result = new TextureInfo();
			var hdPath = texturePath + "$hd";
			var hdFullPath = FlashResources.GetFullPath(hdPath);

			if (FlashResources.useHighDefTextures && ResourceHelper.AssetExists(hdFullPath))
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

			var frameNodes = description.Elements("frame").ToArray();
			var sheetFrames = new SheetFrame[frameNodes.Length];
			var frameIndex = 0;

			foreach (var node in frameNodes)
			{
				var sheet = node.Attribute("sheet");
				var texturePath = sheet != null ? sheet.Value : path;
				var textureInfo = GetTexture(texturePath);
				
				var frame = new SheetFrame(
					textureInfo.texture,
					new Rect(
						Convert.ToInt32(node.Attribute("x").Value),
						Convert.ToInt32(node.Attribute("y").Value),
						Convert.ToInt32(node.Attribute("w").Value),
						Convert.ToInt32(node.Attribute("h").Value)),
					new Vector2(
						Convert.ToInt32(node.Attribute("ax").Value),
						Convert.ToInt32(node.Attribute("ay").Value)),
					textureInfo.scale);

				sheetFrames[frameIndex++] = frame;
			}

			return sheetFrames;
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