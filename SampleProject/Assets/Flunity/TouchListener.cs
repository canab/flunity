using System;
using UnityEngine;
using System.Collections.Generic;
using Flunity.Utils;

namespace Flunity
{
	/// <summary>
	/// Provides events to handle touches on target DisplayObject.
	/// Events will not be fired if target object or its parent is not enabled.
	/// </summary>
	public class TouchListener
	{
		/// <summary>
		/// Rect in local coordinates system which will react on events.
        /// If not specified, <c>DisplayObject.internalBounds</c> will be used
		/// </summary>
		public Rect? hitArea;

		internal bool isRemoved;

		private readonly DisplayObject _target;

		private readonly List<TouchState> _touches = new List<TouchState>();

		private FlashStage _stage;
		private bool _debugDrawEnabled;

		#region events

		/// <summary>
		/// Dispatches when object is touched.
		/// Will be dispatched several times in case of multitouch.
		/// </summary>
		public event Action<TouchListener, TouchState> TouchBegan;

		/// <summary>
		/// Dispatches when touch is ended.
		/// Will be dispatched several times in case of multitouch.
		/// </summary>
		public event Action<TouchListener, TouchState> TouchEnded;

		/// <summary>
		/// Dispatches when touch is canceled.
		/// Will be dispatched several times in case of multitouch.
		/// </summary>
		public event Action<TouchListener, TouchState> TouchCanceled;

		/// <summary>
		/// Dispatches when object is touched first time.
		/// Will be dispatched once in case of multitouch.
		/// </summary>
		public event Action<TouchListener> Pressed;

		/// <summary>
		/// Dispatches when all touches are ended.
		/// Will be dispatched once in case of multitouch.
		/// </summary>
		public event Action<TouchListener> Released;

		/// <summary>
		/// Dispatches when all touches are canceled.
		/// Will be dispatched once in case of multitouch.
		/// </summary>
		public event Action<TouchListener> Canceled;

		#endregion

		#region event helpers

		public TouchListener OnTouchBegan(Action<TouchListener, TouchState> handler)
		{
			TouchBegan += handler;
			return this;
		}

		public TouchListener OnTouchEnded(Action<TouchListener, TouchState> handler)
		{
			TouchEnded += handler;
			return this;
		}

		public TouchListener OnTouchCanceled(Action<TouchListener, TouchState> handler)
		{
			TouchCanceled += handler;
			return this;
		}

		public TouchListener OnPressed(Action<TouchListener> handler)
		{
			Pressed += handler;
			return this;
		}

		public TouchListener OnReleased(Action<TouchListener> handler)
		{
			Released += handler;
			return this;
		}

		public TouchListener OnCanceled(Action<TouchListener> handler)
		{
			Canceled += handler;
			return this;
		}

		#endregion

		#region isPressed

		private bool _isPressed = false;

		public bool isPressed
		{
			get { return _isPressed; }

			private set
			{
				if (_isPressed == value)
					return;

				_isPressed = value;

				if (_isPressed)
					Pressed.Dispatch(this);
				else
					Released.Dispatch(this);
			}
		}
		#endregion

		public TouchListener(DisplayObject target)
		{
			_target = target;
			_target.AddedToStage += OnTargetAddedToStage;
			_target.RemovedFromStage += OnTargetRemovedFromStage;

			if (_target.isOnStage)
				OnTargetAddedToStage(target);
		}

		void OnTargetAddedToStage(DisplayObject obj)
		{
			ClearState();

			_stage = target.stage;
			_stage.touchController.AddListener(this);

			_debugDrawEnabled = Debug.isDebugBuild;
			if (_debugDrawEnabled)
				_stage.drawEvent.AddListener(DrawDebugRect);

		}

		void OnTargetRemovedFromStage(DisplayObject obj)
		{
			ClearState();

			if (_debugDrawEnabled)
				_stage.drawEvent.RemoveListener(DrawDebugRect);

			_stage.touchController.RemoveListener(this);
			_stage = null;
		}

		internal bool HandleTouchBegin(TouchState touch)
		{
			if (isRemoved || !IsTouchEnabled())
				return false;

			if (HasTouch(touch.id))
				return false;

			if (!HitTestPoint(touch.position))
				return false;

			_touches.Add(touch);
			TouchBegan.Dispatch(this, touch);
			isPressed = _touches.Count > 0;

			return true;
		}

		internal void HandleTouchEnd(TouchState touch)
		{
			if (isRemoved || !IsTouchEnabled())
				return;

			if (!HasTouch(touch.id))
				return;

			_touches.RemoveAt(GetTouchIndex(touch.id));
			TouchEnded.Dispatch(this, touch);
			isPressed = _touches.Count > 0;
		}

		internal void HandleTouchCancel(TouchState touch)
		{
			if (isRemoved || !IsTouchEnabled())
				return;

			if (!HasTouch(touch.id))
				return;

			if (HitTestPoint(touch.position))
				return;

			_touches.RemoveAt(GetTouchIndex(touch.id));
			_isPressed = _touches.Count > 0;

			TouchCanceled.Dispatch(this, touch);
			if (_touches.Count == 0)
				Canceled.Dispatch(this);
		}

		internal void refreshTouchState()
		{
			if (!IsTouchEnabled())
				ClearState();
		}

		private bool HasTouch(int touchId)
		{
			for (int i = 0; i < _touches.Count; i++)
			{
				if (_touches[i].id == touchId)
					return true;
			}
			return false;
		}

		private int GetTouchIndex(int touchId)
		{
			for (int i = 0; i < _touches.Count; i++)
			{
				if (_touches[i].id == touchId)
					return i;
			}
			return -1;
		}

		/// <summary>
		/// Returns true if touch area contains point specified in global coordinates.
		/// </summary>
		public bool HitTestPoint(Vector2 globalPoint)
		{
			var localBounds = hitArea.HasValue
				? hitArea.Value
				: target.GetInternalBounds();

			var localPoint = target.GlobalToLocal(globalPoint);

			return localBounds.Contains(localPoint);
		}

		private bool IsTouchEnabled()
		{
			for (var t = target; t != null; t = t.parent)
			{
				if (!t.isTouchEnabled || !t.visible)
					return false;
			}
			return true;
		}

		private void ClearState()
		{
			_isPressed = false;
			_touches.Clear();
		}

		private void DrawDebugRect()
		{
			if (DebugDraw.drawHitAreas && IsTouchEnabled())
			{
				var rect = hitArea.HasValue
					? hitArea.Value
					: target.GetInternalBounds();

				DebugDraw.DrawRect(target, rect, DebugDraw.drawHitAreasColor);
			}
		}

		/// <summary>
		/// Returns all touches are active at the moment. Does not allocate memory.
		/// </summary>
		public ICollection<TouchState> touches
		{
			get { return _touches; }
		}

		public DisplayObject target
		{
			get { return _target; }
		}
	}
}

