using UnityEngine;
using System;

namespace Flunity
{
	/// <summary>
	/// Invisible display object wich can handle touches.
	/// Change size and position to set touch area.
	/// </summary>
	public class TouchRect : DisplayObject
	{
		private Vector2 _size = new Vector2(100, 100);
		private readonly TouchListener _touchListenter;

		public TouchRect()
		{
			_touchListenter = new TouchListener(this);
		}
		
		public TouchRect(DisplayContainer parent) : this()
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
		}

		public override Vector2 size
		{
			get { return _size; }
			set { _size = value; }
		}

		public TouchListener touchListener
		{
			get { return _touchListenter; }
		}

		#region TouchListenter delegates

		public event Action<TouchListener, TouchState> TouchBegan
		{
			add { _touchListenter.TouchBegan += value; }
			remove { _touchListenter.TouchBegan -= value; }
		}

		public event Action<TouchListener, TouchState> TouchEnded
		{
			add { _touchListenter.TouchEnded += value; }
			remove { _touchListenter.TouchEnded -= value; }
		}

		public event Action<TouchListener> Pressed
		{
			add { _touchListenter.Pressed += value; }
			remove { _touchListenter.Pressed -= value; }
		}

		public event Action<TouchListener> Released
		{
			add { _touchListenter.Released += value; }
			remove { _touchListenter.Released -= value; }
		}

		public TouchListener OnTouchBegan(Action<TouchListener, TouchState> handler)
		{
			_touchListenter.TouchBegan += handler;
			return _touchListenter;
		}

		public TouchListener OnTouchEnded(Action<TouchListener, TouchState> handler)
		{
			_touchListenter.TouchEnded += handler;
			return _touchListenter;
		}

		public TouchListener OnTouchCanceled(Action<TouchListener, TouchState> handler)
		{
			_touchListenter.TouchCanceled += handler;
			return _touchListenter;
		}

		public TouchListener OnPressed(Action<TouchListener> handler)
		{
			_touchListenter.Pressed += handler;
			return _touchListenter;
		}

		public TouchListener OnReleased(Action<TouchListener> handler)
		{
			_touchListenter.Released += handler;
			return _touchListenter;
		}

		public TouchListener OnCanceled(Action<TouchListener> handler)
		{
			_touchListenter.Canceled += handler;
			return _touchListenter;
		}

		public bool isPressed
		{
			get { return _touchListenter.isPressed; }
		}

		#endregion
	}
}
