using System;
using UnityEngine;

namespace Flunity.Internal
{
	/// <summary>
	/// Holds cached quads which will be drawn.
	/// </summary>
	public class QuadCollection
	{
		private const int DEFAULT_SIZE = 8;

		internal SpriteQuad[] quads;
		internal int quadsCount = 0;

		public QuadCollection() : this(DEFAULT_SIZE)
		{}

		public QuadCollection(int size)
		{
			quads = new SpriteQuad[size];
		}

		/// <summary>
		/// Clears collection.
		/// </summary>
		public void Clear()
		{
			quadsCount = 0;
		}

		/// <summary>
		/// Adds quad to collection.
		/// </summary>
		public void Add(SpriteQuad spriteQuad)
		{
			EnsureSize(quadsCount + 1);
			quads[quadsCount] = spriteQuad;
			quadsCount += 1;
		}

		/// <summary>
		/// Updates matrix for each quad.
		/// </summary>
		public void UpdateTransform(int quadIndex, ref Matrix4x4 matrix, ref Rect textureRect, ref Vector2 textureSize, ref Vector2 anchor, TextureFlip flip)
		{
			EnsureSize(quadIndex + 1);
			quads[quadIndex].UpdateTransform(ref matrix, ref textureRect, ref textureSize, ref anchor, flip);
		}

		/// <summary>
		/// Updates color transform for each quad.
		/// </summary>
		public void UpdateColor(ref Color color)
		{
			for (int i = 0; i < quadsCount; i++)
			{
				quads[i].UpdateColor(ref color);
			}			
		}

		/// <summary>
		/// Updates color transform for each quad.
		/// </summary>
		public void UpdateColor(ref ColorTransform color)
		{
			for (int i = 0; i < quadsCount; i++)
			{
				quads[i].UpdateColor(ref color);
			}			
		}

		private void EnsureSize(int requiredSize)
		{
			if (requiredSize <= quads.Length)
				return;

			var newSize = quads.Length * 2;
			while (newSize < requiredSize)
			{
				newSize *= 2;
			}

			Array.Resize(ref quads, newSize);
		}
	}
}