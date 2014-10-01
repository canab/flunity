using UnityEngine;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Flags to draw some gizmos like touch areas.
	/// </summary>
	public static class DebugDraw
	{
		public static bool drawHitAreas = false;
		public static Color drawHitAreasColor = Color.cyan;

		public static bool drawPlaceholders = false;
		public static Color drawPlaceholdersColor = Color.red;

		public static bool drawTextField = false;
		public static Color drawTextFieldColor = Color.green;

		static DebugDraw()
		{
			drawHitAreasColor.a = 0.7f;
			drawTextFieldColor.a = 0.7f;
		}

		private static Texture2D _debugTexture;

		public static void DrawRect(DisplayObject target, Rect rect, Color color)
		{
			if (_debugTexture == null)
			{
				_debugTexture = new Texture2D(1, 1);
				_debugTexture.SetPixel(0, 0, Color.white);
			}

			var textureSize = new Vector2(1, 1);
			var matrix = MatrixUtil.Create2D(new Vector2(rect.width, rect.height),
				0, new Vector2(rect.x, rect.y));
			MatrixUtil.Multiply(ref matrix, ref target.compositeMatrix, out matrix);

			var quad = new SpriteQuad();
			quad.UpdateColor(ref color);
			quad.UpdateTransform(ref matrix, ref textureSize);
			quad.SetZOrder(-10);

			target.stage.debugBatch.DrawQuad(_debugTexture, ref quad);
		}
	}
}