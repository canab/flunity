using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Holds info about single frame in the sprite sheet
	/// </summary>
	public class SheetFrame
	{
		/// <summary>
		/// Sprite sheet texture
		/// </summary>
		public readonly Texture2D texture;

		/// <summary>
		/// Bounds of the whool texture (probably legacy data)
		/// </summary>
		public readonly Rect textureBounds;

		/// <summary>
		/// Scale to be adjustad on HD textures
		/// </summary>
		public readonly float textureScale;

		/// <summary>
		/// Anchor point of the sprite (registration point)
		/// </summary>
		public readonly Vector2 textureAnchor;

		/// <summary>
		/// Rectangle with the sprite in the sprite sheet
		/// </summary>
		public readonly Rect bounds;

		public SheetFrame(Texture2D texture)
			:this(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero)
		{
		}

		public SheetFrame(Texture2D texture, Vector2 textureAnchor, float textureScale = 1)
			:this(texture, new Rect(0, 0, texture.width, texture.height), textureAnchor, textureScale)
		{
		}

		public SheetFrame(Texture2D texture, Rect textureBounds, Vector2 textureAnchor, float textureScale = 1)
		{
			this.texture = texture;
			this.textureBounds = textureBounds;
			this.textureAnchor = textureAnchor;
			this.textureScale = textureScale;

			bounds = new Rect(
				-textureAnchor.x * textureScale,
				-textureAnchor.y * textureScale,
				textureBounds.width * textureScale,
				textureBounds.height * textureScale);
		}

		public float width
		{
			get { return bounds.width; }
		}
		
		public Vector2 size
		{
			get { return new Vector2(bounds.width, bounds.height); }
		}
		
		public float height
		{
			get { return bounds.height; }
		}

		public bool isEmpty
		{
			get { return textureBounds.width < 0.5f || textureBounds.height  < 0.5f; }
		}
	}
}