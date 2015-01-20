using System;
using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Struct holds full color and tint info.
	/// </summary>
	public struct ColorTransform
	{
		public static void Compose(ref ColorTransform a, ref ColorTransform b, out ColorTransform result)
		{
			result.rMult = a.rMult * b.rMult;
			result.gMult = a.gMult * b.gMult;
			result.bMult = a.bMult * b.bMult;
			result.aMult = a.aMult * b.aMult;

			result.rOffset = a.rOffset + b.rOffset;
			result.gOffset = a.gOffset + b.gOffset;
			result.bOffset = a.bOffset + b.bOffset;
			result.aOffset = a.aOffset + b.aOffset;
		}

		public static ColorTransform Tint(Color color, float mult)
		{
			return Tint(color.r, color.b, color.g, mult);
		}

		public static ColorTransform Tint(float r, float g, float b, float mult)
		{
			return new ColorTransform(
				1 - mult,
				1 - mult,
				1 - mult,
				1,
				mult * r,
				mult * g,
				mult * b,
				0);
		}

		public float rMult;
		public float gMult;
		public float bMult;
		public float aMult;

		public float rOffset;
		public float gOffset;
		public float bOffset;
		public float aOffset;

		public ColorTransform(float rMult = 1, float gMult = 1, float bMult = 1, float aMult = 1,
			float rOffset = 0, float gOffset = 0, float bOffset = 0, float aOffset = 0)
		{
			this.rMult = rMult;
			this.gMult = gMult;
			this.bMult = bMult;
			this.aMult = aMult;

			this.rOffset = rOffset;
			this.gOffset = gOffset;
			this.bOffset = bOffset;
			this.aOffset = aOffset;
		}

		public ColorTransform(Color color)
		{
			this.rMult = color.r;
			this.gMult = color.g;
			this.bMult = color.b;
			this.aMult = color.a;

			this.rOffset = 0;
			this.gOffset = 0;
			this.bOffset = 0;
			this.aOffset = 0;
		}

		public float GetBrightness()
		{
			return (0.299f * rMult + 0.587f * gMult + 0.114f * bMult);
		}

		public void SetBrightness(float value)
		{
			var brightness = GetBrightness();

			if (Math.Abs(brightness) < 1e-6)
			{
				rMult = bMult = gMult = value;
			}
			else
			{
				rMult = rMult / brightness * value;
				gMult = gMult / brightness * value;
				bMult = bMult / brightness * value;
			}
		}

		public Color GetColor()
		{
			return new Color(rMult, gMult, bMult, aMult);
		}

		public void GetColor(out Color color)
		{
			color.r = rMult;
			color.g = gMult;
			color.b = bMult;
			color.a = aMult;
		}

		public void GetColor(out Color32 color)
		{
			color.r = (byte) (rMult * 255f);
			color.g = (byte) (gMult * 255f);
			color.b = (byte) (bMult * 255f);
			color.a = (byte) (aMult * 255f);
		}

		public void SetColor(Color color)
		{
			rMult = color.r;
			gMult = color.g;
			bMult = color.b;
			aMult = color.a;
		}

		public void SetColor(ref Color color)
		{
			rMult = color.r;
			gMult = color.g;
			bMult = color.b;
			aMult = color.a;
		}

		public Color GetTint()
		{
			return new Color(rOffset, gOffset, bOffset, aOffset);
		}

		public void GetTint(out Color color)
		{
			color.r = rOffset;
			color.g = gOffset;
			color.b = bOffset;
			color.a = aOffset;
		}

		internal void GetTint(out Vector4 color)
		{
			color.x = rOffset;
			color.y = gOffset;
			color.z = bOffset;
			color.w = aOffset;
		}

		internal void GetTint(out Vector2 holder)
		{
			holder.x = rOffset + 1 + (int)(gOffset * 255);
			holder.y = bOffset + 1 + (int)(aOffset * 255);
		}

		public void GetTint(out Color32 color)
		{
			color.r = (byte) (rOffset * 255f);
			color.g = (byte) (gOffset * 255f);
			color.b = (byte) (bOffset * 255f);
			color.a = (byte) (aOffset * 255f);
		}

		public void SetTint(Color color)
		{
			rOffset = color.r;
			gOffset = color.g;
			bOffset = color.b;
			aOffset = color.a;
		}

		public void SetTint(ref Color color)
		{
			rOffset = color.r;
			gOffset = color.g;
			bOffset = color.b;
			aOffset = color.a;
		}

		public override string ToString()
		{
			return string.Format("[ColorTransform: {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}]",
				rMult, gMult, bMult, aMult, rOffset, gOffset, bOffset, aOffset);
		}
		
	}
}

