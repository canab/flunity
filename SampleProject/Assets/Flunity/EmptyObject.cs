using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Invisible display object.
	/// Purpose is to implement some kinds of placeholders.
	/// </summary>
	public class EmptyObject : DisplayObject
	{
		private Vector2 _size = new Vector2(100, 100);

		public EmptyObject()
		{
		}

		public EmptyObject(DisplayContainer parent) : this()
		{
			this.parent = parent;
		}

		public override Rect GetInternalBounds()
		{
			var a = anchor;
			return new Rect(-a.x, -a.y, _size.x, _size.y);
		}

		public override void Draw()
		{
			if (DebugDraw.drawPlaceholders)
				DebugDraw.DrawRect(this, GetInternalBounds(), DebugDraw.drawPlaceholdersColor);
		}

		public override Vector2 size
		{
			get { return _size; }
			set { _size = value; }
		}
	}
}


