using UnityEngine;
using System;

namespace Flunity.Utils
{
	/// <summary>
	/// Helper methods
	/// </summary>
	public static class MatrixUtil
	{
		public static Matrix4x4 CreateScale2D(float scale)
		{
			Matrix4x4 result;
			CreateScale(scale, scale, scale, out result);
			return result;
		}

		public static Matrix4x4 CreateScale2D(float scaleX, float scaleY)
		{
			Matrix4x4 result;
			CreateScale(scaleX, scaleY, 1, out result);
			return result;
		}

		public static void CreateScale(float x, float y, float z, out Matrix4x4 result)
		{
			result.m00 = x;
			result.m01 = 0;
			result.m02 = 0;
			result.m03 = 0;
			result.m10 = 0;
			result.m11 = y;
			result.m12 = 0;
			result.m13 = 0;
			result.m20 = 0;
			result.m21 = 0;
			result.m22 = z;
			result.m23 = 0;
			result.m30 = 0;
			result.m31 = 0;
			result.m32 = 0;
			result.m33 = 1;
		}

		public static Matrix4x4 Create2D(Vector2 scale, float rotation, Vector2 translation)
		{
			Matrix4x4 result;
			Create2D(ref scale, rotation, ref translation, out result);
			return result;
		}

		public static void Create2D(ref Vector2 scale, float rotation, ref Vector2 translation,
		                            out Matrix4x4 result)
		{
			// Analysis disable CompareOfFloatsByEqualityOperator
			if (rotation == 0)
			// Analysis restore CompareOfFloatsByEqualityOperator
			{
				result.m00 = scale.x;
				result.m01 = 0;

				result.m10 = 0;
				result.m11 = scale.y;
			}
			else
			{
				var sin = (float)Math.Sin(rotation);
				var cos = (float)Math.Cos(rotation);

				result.m00 = scale.x * cos;
				result.m01 = scale.x * sin;

				result.m10 = -scale.y * sin;
				result.m11 = scale.y * cos;
			}

			result.m02 = 0;
			result.m03 = 0;

			result.m12 = 0;
			result.m13 = 0;

			result.m20 = 0;
			result.m21 = 0;
			result.m22 = 1;
			result.m23 = 0;

			result.m30 = translation.x;
			result.m31 = translation.y;
			result.m32 = 0;
			result.m33 = 1;
		}

		public static void Multiply(ref Matrix4x4 a, ref Matrix4x4 b, out Matrix4x4 result)
		{
			var m11 = (((a.m00 * b.m00) + (a.m01 * b.m10)) + (a.m02 * b.m20)) + (a.m03 * b.m30);
			var m12 = (((a.m00 * b.m01) + (a.m01 * b.m11)) + (a.m02 * b.m21)) + (a.m03 * b.m31);
			var m13 = (((a.m00 * b.m02) + (a.m01 * b.m12)) + (a.m02 * b.m22)) + (a.m03 * b.m32);
			var m14 = (((a.m00 * b.m03) + (a.m01 * b.m13)) + (a.m02 * b.m23)) + (a.m03 * b.m33);
			var m21 = (((a.m10 * b.m00) + (a.m11 * b.m10)) + (a.m12 * b.m20)) + (a.m13 * b.m30);
			var m22 = (((a.m10 * b.m01) + (a.m11 * b.m11)) + (a.m12 * b.m21)) + (a.m13 * b.m31);
			var m23 = (((a.m10 * b.m02) + (a.m11 * b.m12)) + (a.m12 * b.m22)) + (a.m13 * b.m32);
			var m24 = (((a.m10 * b.m03) + (a.m11 * b.m13)) + (a.m12 * b.m23)) + (a.m13 * b.m33);
			var m31 = (((a.m20 * b.m00) + (a.m21 * b.m10)) + (a.m22 * b.m20)) + (a.m23 * b.m30);
			var m32 = (((a.m20 * b.m01) + (a.m21 * b.m11)) + (a.m22 * b.m21)) + (a.m23 * b.m31);
			var m33 = (((a.m20 * b.m02) + (a.m21 * b.m12)) + (a.m22 * b.m22)) + (a.m23 * b.m32);
			var m34 = (((a.m20 * b.m03) + (a.m21 * b.m13)) + (a.m22 * b.m23)) + (a.m23 * b.m33);
			var m41 = (((a.m30 * b.m00) + (a.m31 * b.m10)) + (a.m32 * b.m20)) + (a.m33 * b.m30);
			var m42 = (((a.m30 * b.m01) + (a.m31 * b.m11)) + (a.m32 * b.m21)) + (a.m33 * b.m31);
			var m43 = (((a.m30 * b.m02) + (a.m31 * b.m12)) + (a.m32 * b.m22)) + (a.m33 * b.m32);
			var m44 = (((a.m30 * b.m03) + (a.m31 * b.m13)) + (a.m32 * b.m23)) + (a.m33 * b.m33);
			result.m00 = m11;
			result.m01 = m12;
			result.m02 = m13;
			result.m03 = m14;
			result.m10 = m21;
			result.m11 = m22;
			result.m12 = m23;
			result.m13 = m24;
			result.m20 = m31;
			result.m21 = m32;
			result.m22 = m33;
			result.m23 = m34;
			result.m30 = m41;
			result.m31 = m42;
			result.m32 = m43;
			result.m33 = m44;
		}

		public static void Inverse(ref Matrix4x4 m, out Matrix4x4 result)
		{
			float n1 = m.m00;
			float n2 = m.m01;
			float n3 = m.m02;
			float n4 = m.m03;
			float n5 = m.m10;
			float n6 = m.m11;
			float n7 = m.m12;
			float n8 = m.m13;
			float n9 = m.m20;
			float n10 = m.m21;
			float n11 = m.m22;
			float n12 = m.m23;
			float n13 = m.m30;
			float n14 = m.m31;
			float n15 = m.m32;
			float n16 = m.m33;
			float n17 = (float)((double)n11 * (double)n16 - (double)n12 * (double)n15);
			float n18 = (float)((double)n10 * (double)n16 - (double)n12 * (double)n14);
			float n19 = (float)((double)n10 * (double)n15 - (double)n11 * (double)n14);
			float n20 = (float)((double)n9 * (double)n16 - (double)n12 * (double)n13);
			float n21 = (float)((double)n9 * (double)n15 - (double)n11 * (double)n13);
			float n22 = (float)((double)n9 * (double)n14 - (double)n10 * (double)n13);
			float n23 = (float)((double)n6 * (double)n17 - (double)n7 * (double)n18 + (double)n8 * (double)n19);
			float n24 = (float)-((double)n5 * (double)n17 - (double)n7 * (double)n20 + (double)n8 * (double)n21);
			float n25 = (float)((double)n5 * (double)n18 - (double)n6 * (double)n20 + (double)n8 * (double)n22);
			float n26 = (float)-((double)n5 * (double)n19 - (double)n6 * (double)n21 + (double)n7 * (double)n22);
			float n27 = (float)(1.0 / ((double)n1 * (double)n23 + (double)n2 * (double)n24 + (double)n3 * (double)n25 + (double)n4 * (double)n26));

			result.m00 = n23 * n27;
			result.m10 = n24 * n27;
			result.m20 = n25 * n27;
			result.m30 = n26 * n27;
			result.m01 = (float)-((double)n2 * (double)n17 - (double)n3 * (double)n18 + (double)n4 * (double)n19) * n27;
			result.m11 = (float)((double)n1 * (double)n17 - (double)n3 * (double)n20 + (double)n4 * (double)n21) * n27;
			result.m21 = (float)-((double)n1 * (double)n18 - (double)n2 * (double)n20 + (double)n4 * (double)n22) * n27;
			result.m31 = (float)((double)n1 * (double)n19 - (double)n2 * (double)n21 + (double)n3 * (double)n22) * n27;

			float n28 = (float)((double)n7 * (double)n16 - (double)n8 * (double)n15);
			float n29 = (float)((double)n6 * (double)n16 - (double)n8 * (double)n14);
			float n30 = (float)((double)n6 * (double)n15 - (double)n7 * (double)n14);
			float n31 = (float)((double)n5 * (double)n16 - (double)n8 * (double)n13);
			float n32 = (float)((double)n5 * (double)n15 - (double)n7 * (double)n13);
			float n33 = (float)((double)n5 * (double)n14 - (double)n6 * (double)n13);

			result.m02 = (float)((double)n2 * (double)n28 - (double)n3 * (double)n29 + (double)n4 * (double)n30) * n27;
			result.m12 = (float)-((double)n1 * (double)n28 - (double)n3 * (double)n31 + (double)n4 * (double)n32) * n27;
			result.m22 = (float)((double)n1 * (double)n29 - (double)n2 * (double)n31 + (double)n4 * (double)n33) * n27;
			result.m32 = (float)-((double)n1 * (double)n30 - (double)n2 * (double)n32 + (double)n3 * (double)n33) * n27;

			float n34 = (float)((double)n7 * (double)n12 - (double)n8 * (double)n11);
			float n35 = (float)((double)n6 * (double)n12 - (double)n8 * (double)n10);
			float n36 = (float)((double)n6 * (double)n11 - (double)n7 * (double)n10);
			float n37 = (float)((double)n5 * (double)n12 - (double)n8 * (double)n9);
			float n38 = (float)((double)n5 * (double)n11 - (double)n7 * (double)n9);
			float n39 = (float)((double)n5 * (double)n10 - (double)n6 * (double)n9);

			result.m03 = (float)-((double)n2 * (double)n34 - (double)n3 * (double)n35 + (double)n4 * (double)n36) * n27;
			result.m13 = (float)((double)n1 * (double)n34 - (double)n3 * (double)n37 + (double)n4 * (double)n38) * n27;
			result.m23 = (float)-((double)n1 * (double)n35 - (double)n2 * (double)n37 + (double)n4 * (double)n39) * n27;
			result.m33 = (float)((double)n1 * (double)n36 - (double)n2 * (double)n38 + (double)n3 * (double)n39) * n27;
		}

		public static void TransformPos(ref Vector2 pos, ref Matrix4x4 matrix, out Vector2 result)
		{
			result = new Vector2(
				(pos.x * matrix.m00) + (pos.y * matrix.m10) + matrix.m30,
				(pos.x * matrix.m01) + (pos.y * matrix.m11) + matrix.m31);
		}
	}
}

