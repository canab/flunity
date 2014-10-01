using System;
using UnityEngine;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class GeomUtil
	{
		/// <summary>
		/// Returns true if lines are intersecting
		/// </summary>
		public static Boolean AreLinesIntersect(ref Vector2 a1, ref Vector2 a2,
		                                        ref Vector2 b1, ref Vector2 b2)
		{
			var v1 = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
			var v2 = (b2.x - b1.x) * (a2.y - b1.y) - (b2.y - b1.y) * (a2.x - b1.x);
			var v3 = (a2.x - a1.x) * (b1.y - a1.y) - (a2.y - a1.y) * (b1.x - a1.x);
			var v4 = (a2.x - a1.x) * (b2.y - a1.y) - (a2.y - a1.y) * (b2.x - a1.x);

			return (v1 * v2 <= 0) && (v3 * v4 <= 0);
		}

		/// <summary>
		/// Returns true if lines are intersecting
		/// </summary>
		public static Boolean AreLinesIntersect(Vector2 a1, Vector2 a2,
												Vector2 b1, Vector2 b2)
		{
			var v1 = (b2.x - b1.x) * (a1.y - b1.y) - (b2.y - b1.y) * (a1.x - b1.x);
			var v2 = (b2.x - b1.x) * (a2.y - b1.y) - (b2.y - b1.y) * (a2.x - b1.x);
			var v3 = (a2.x - a1.x) * (b1.y - a1.y) - (a2.y - a1.y) * (b1.x - a1.x);
			var v4 = (a2.x - a1.x) * (b2.y - a1.y) - (a2.y - a1.y) * (b2.x - a1.x);

			return (v1 * v2 <= 0) && (v3 * v4 <= 0);
		}
		
		/// <summary>
		/// Returns an angle between X axis and vector specified by two points
		/// </summary>
		public static double GetAngle(Vector2 startPoint, Vector2 endPoint)
		{
			var dx = endPoint.x - startPoint.x;
			var dy = endPoint.y - startPoint.y;

			return Math.Atan2(dy,  dx);
		}
		
		/// <summary>
		/// Returns an angle between two vectors
		/// </summary>
		public static double GetAngleDiff(Vector2 sourceVec, Vector2 destVec)
		{
			var angle1 = Math.Atan2(sourceVec.y, sourceVec.x);
			var angle2 = Math.Atan2(destVec.y, destVec.x);

			return MathUtil.AngleDiff(angle1, angle2);
		}

		/// <summary>
		/// Returns the size of the Rect
		/// </summary>
		public static Vector2 SizeVector(this Rect value)
		{
			return new Vector2(value.width, value.height);
		}
		
		/// <summary>
		/// Returns the TopLeft point of the Rect
		/// </summary>
		public static Vector2 PosVector(this Rect value)
		{
			return new Vector2(value.x, value.y);
		}

		/// <summary>
		/// Returns Rect with size of specified value
		/// </summary>
		public static Rect ToRect(this Vector2 value)
		{
			return new Rect(0, 0, value.x.RoundToInt(), value.y.RoundToInt());
		}
		
		/// <summary>
		/// Returns vector with rounded members
		/// </summary>
		public static Vector2 RoundToInt(this Vector2 value)
		{
			return new Vector2(value.x.RoundToInt(), value.y.RoundToInt());
		}
		
		/// <summary>
		/// Transforms polar coordinates to decart
		/// </summary>
		public static Vector2 FromPolar(float magnitude, float angle)
		{
			return new Vector2(
				magnitude * (float) Math.Cos(angle),
				magnitude * (float) Math.Sin(angle));
		}

		/// <summary>
		/// Returns true if rect contains specified point
		/// </summary>
		public static bool Contains(this Rect rect, Vector2 point)
		{
			return Contains(rect, point.x, point.y);
		}

		/// <summary>
		/// Returns true if rect contains specified point
		/// </summary>
		public static bool Contains(this Rect rect, float x, float y)
		{
			return rect.x <= x && x < rect.x + rect.width
				&& rect.y <= y && y < rect.y + rect.height;
		}

		private static readonly Vector2[] _tmpVec2Array = new Vector2[4];

		/// <summary>
		/// Transforms bounds according to transformation matrix
		/// </summary>
		public static void GetBounds(ref Rect rect, ref Matrix4x4 matrix, out Rect result)
		{
			_tmpVec2Array[0] = new Vector2(rect.xMin, rect.yMin);
			_tmpVec2Array[1] = new Vector2(rect.xMax, rect.yMin);
			_tmpVec2Array[2] = new Vector2(rect.xMax, rect.yMax);
			_tmpVec2Array[3] = new Vector2(rect.xMin, rect.yMax);

			float minX = int.MaxValue;
			float minY = int.MaxValue;
			float maxX = int.MinValue;
			float maxY = int.MinValue;

			for (int i = 0; i < 4; i++)
			{
				Vector2 globalPoint;
				MatrixUtil.TransformPos(ref _tmpVec2Array[i], ref matrix, out globalPoint);

				if (globalPoint.x < minX)
					minX = globalPoint.x;

				if (globalPoint.y < minY)
					minY = globalPoint.y;

				if (globalPoint.x > maxX)
					maxX = globalPoint.x;

				if (globalPoint.y > maxY)
					maxY = globalPoint.y;
			}

			result = new Rect(minX, minY, maxX - minX, maxY - minY);
		}

		/// <summary>
		/// Returns summary bounds of two rectangles.
		/// </summary>
		public static Rect UnionRect(this Rect rect1, Rect rect2)
		{
			var result = new Rect();
			UnionRect(ref rect1, ref rect2, out result);
			return result;
		}

		/// <summary>
		/// Returns summary bounds of two rectangles.
		/// </summary>
		public static void UnionRect(ref Rect rect1, ref Rect rect2, out Rect result)
		{
			var left = Math.Min(rect1.x, rect2.x);
			var top = Math.Min(rect1.y, rect2.y);
			var right = Math.Max(rect1.xMax, rect2.xMax);
			var bottom = Math.Max(rect1.yMax, rect2.yMax);

			result = new Rect(left, top, right - left, bottom - top);
		}

		/// <summary>
		/// Returns true if two rectangles are intersecting.
		/// </summary>
		public static bool Intersects(ref Rect a, ref Rect b)
		{
			return a.xMin < b.xMax
				&& b.xMin < a.xMax
				&& a.yMin < b.yMax
				&& b.yMin < a.yMax;
		}

		/// <summary>
		/// Returns true if two rectangles are intersecting.
		/// </summary>
		public static bool Intersects(this Rect a, Rect b)
		{
			return Intersects(ref a, ref b);
		}

		/// <summary>
		/// Returns an intersection of two rectangles
		/// </summary>
		public static Rect GetIntersection(this Rect a, Rect b)
		{
			Rect rectangle;
			GetIntersection(ref a, ref b, out rectangle);
			return rectangle;
		}

		/// <summary>
		/// Returns an intersection of two rectangles
		/// </summary>
		public static void GetIntersection(ref Rect a, ref Rect b, out Rect result)
		{
			if (Intersects(ref a, ref b))
			{
				float right = Math.Min(a.x + a.width, b.x + b.width);
				float left = Math.Max(a.x, b.x);
				float top = Math.Max(a.y, b.y);
				float bottom = Math.Min(a.y + a.height, b.y + b.height);
				result = new Rect(left, top, right - left, bottom - top);
			}
			else
			{
				result = new Rect(0, 0, 0, 0);
			}
		}
	}
}