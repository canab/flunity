using System;
using UnityEngine;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity.Internal
{
	/// <summary>
	/// Holds data of 4 vertices of the sprite
	/// </summary>
	public struct SpriteQuad
	{
		public VertexData leftTop;
		public VertexData rightTop;
		public VertexData leftBottom;
		public VertexData rightBottom;

		public void UpdateTransform(ref Matrix4x4 matrix, ref Vector2 textureSize)
		{
			var textureRect = new Rect(0, 0, textureSize.x, textureSize.y);
			var anchor = Vector2.zero;

			UpdateVertices(ref matrix, ref textureRect, ref anchor);
			UpdateTexture(ref textureRect, ref textureSize, TextureFlip.NONE);
		}

		public void UpdateTransform(ref Matrix4x4 matrix, ref Rect textureRect, ref Vector2 textureSize,
			ref Vector2 anchor, TextureFlip flip = TextureFlip.NONE)
		{
			UpdateVertices(ref matrix, ref textureRect, ref anchor);
			UpdateTexture(ref textureRect, ref textureSize, flip);
		}

		private void UpdateVertices(ref Matrix4x4 matrix, ref Rect textureRect, ref Vector2 anchor)
		{
			var localLt = new Vector2(-anchor.x, -anchor.y);
			var localRb = new Vector2(textureRect.width - anchor.x, textureRect.height - anchor.y);
			var localRt = new Vector2(localRb.x, localLt.y);
			var localLb = new Vector2(localLt.x, localRb.y);

			Vector2 globalLt;
			Vector2 globalRt;
			Vector2 globalLb;
			Vector2 globalRb;

			MatrixUtil.TransformPos(ref localLt, ref matrix, out globalLt);
			MatrixUtil.TransformPos(ref localRt, ref matrix, out globalRt);
			MatrixUtil.TransformPos(ref localLb, ref matrix, out globalLb);
			MatrixUtil.TransformPos(ref localRb, ref matrix, out globalRb);

			leftTop.position = new Vector3(globalLt.x, globalLt.y, 0);
			rightTop.position = new Vector3(globalRt.x, globalRt.y, 0);
			leftBottom.position = new Vector3(globalLb.x, globalLb.y, 0);
			rightBottom.position = new Vector3(globalRb.x, globalRb.y, 0);
		}

		public void SetZOrder(float z)
		{
			leftTop.position.z = z;
			rightTop.position.z = z;
			leftBottom.position.z = z;
			rightBottom.position.z = z;
		}

		public Rect GetBounds()
		{
			var x = MathUtil.Min(
				leftTop.position.x, rightTop.position.x,
				leftBottom.position.x, rightBottom.position.x);

			var y = MathUtil.Min(
				leftTop.position.y, rightTop.position.y,
				leftBottom.position.y, rightBottom.position.y);

			var right = MathUtil.Max(
				leftTop.position.x, rightTop.position.x,
				leftBottom.position.x, rightBottom.position.x);

			var bottom = MathUtil.Max(
				leftTop.position.y, rightTop.position.y,
				leftBottom.position.y, rightBottom.position.y);

			return new Rect(x, y, right - x, bottom - y);
		}

		private void UpdateTexture(ref Rect textureRect, ref Vector2 textureSize, TextureFlip flip)
		{
			var lt = new Vector2(textureRect.x / textureSize.x, textureRect.y / textureSize.y);
			var rb = new Vector2(textureRect.xMax / textureSize.x, textureRect.yMax / textureSize.y);

			if ((flip & TextureFlip.HORIZONTAL) != 0)
			{
				var temp = rb.x;
				rb.x = lt.x;
				lt.x = temp;
			}

			if ((flip & TextureFlip.VERTICAL) != 0)
			{
				var temp = rb.y;
				rb.y = lt.y;
				lt.y = temp;
			}

			leftTop.texCoord = new Vector2(lt.x, 1 - lt.y);
			rightBottom.texCoord = new Vector2(rb.x, 1 - rb.y);
			leftBottom.texCoord = new Vector2(lt.x, 1 - rb.y);
			rightTop.texCoord = new Vector2(rb.x, 1 - lt.y);
		}

		public void UpdateColor(ref ColorTransform color)
		{
			leftTop.color = color;
			rightTop.color = color;
			leftBottom.color = color;
			rightBottom.color = color;
		}

		public void UpdateColor(ref Color color)
		{
			leftTop.color.SetColor(ref color);
			rightTop.color.SetColor(ref color);
			leftBottom.color.SetColor(ref color);
			rightBottom.color.SetColor(ref color);
		}
	}
}