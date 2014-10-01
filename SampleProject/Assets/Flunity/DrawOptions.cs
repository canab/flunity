namespace Flunity
{
	/// <summary>
	/// <b>Does not implemented!</b>
	/// (Should add ability to set custom draw options for certain DisplayObjects).
	/// </summary>
	public class DrawOptions
	{
		public static readonly DrawOptions DEFAULT = new DrawOptions();

		public object blendState;
		public object shader;

		public void Reset()
		{
			blendState = null;
			shader = null;
		}

		public void CopyFrom(DrawOptions x)
		{
			blendState = x.blendState;
			shader = x.shader;
		}

		public void MergeWith(DrawOptions x)
		{
			if (blendState == null)
				blendState = x.blendState;

			if (shader == null)
				shader = x.shader;
		}

		public bool EqualsTo(DrawOptions x)
		{
			return x != null
				&& (x == this)
				|| (blendState == x.blendState && shader == x.shader);
		}
	}
}