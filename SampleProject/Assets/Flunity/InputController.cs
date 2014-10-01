using System;
using System.Collections.Generic;
using Flunity.Common;
using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Provides unified interface to handle global events both from touch device and mouse.
	/// To handle events on local objects see TouchListener, TouchRect
	/// </summary>
	public class InputController
	{
		#region touchBegan

		private readonly EventSender<TouchState> _touchBegan = new EventSender<TouchState>();

		public event Action<TouchState> TouchBegan
		{
			add { _touchBegan.AddListener(value); }
			remove { _touchBegan.RemoveListener(value); }
		}
		#endregion

		#region touchEnded

		private readonly EventSender<TouchState> _touchEnded = new EventSender<TouchState>();

		public event Action<TouchState> TouchEnded
		{
			add { _touchEnded.AddListener(value); }
			remove { _touchEnded.RemoveListener(value); }
		}

		#endregion

		private readonly FlashStage _component;

		public InputController(FlashStage component)
		{
			_component = component;
		}
		
		private readonly List<TouchState> _touchStates = new List<TouchState>(8);
		private readonly List<TouchState> _newStates = new List<TouchState>(8);

		protected void ApplyNewStates()
		{
			foreach (var newState in _newStates)
			{
				var existingNum = GetStateIndex(_touchStates, newState.id);
				if (existingNum >= 0)
				{
					_touchStates[existingNum] = newState;
				}
				else
				{
					_touchStates.Add(newState);
					_touchBegan.Dispatch(newState);
				}
			}

			if (_touchStates.Count == _newStates.Count)
				return;

			for (int i = _touchStates.Count - 1; i >= 0; i--)
			{
				var curState = _touchStates[i];
				var existingNum = GetStateIndex(_newStates, curState.id);
				if (existingNum < 0)
				{
					_touchStates.RemoveAt(i);
					_touchEnded.Dispatch(curState);
				}
			}
		}

		private int GetStateIndex(List<TouchState> collection, int stateId)
		{
			var count = collection.Count;
			for (int i = 0; i < count; i++)
			{
				if (collection[i].id == stateId)
					return i;
			}

			return -1;
		}

		public TouchState? GetState(int stateId)
		{
			foreach (var state in _touchStates)
			{
				if (state.id == stateId)
					return state;
			}

			return null;
		}
		
		public void DoUpdate()
		{
			_newStates.Clear();

			#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
				ReadMouse();
			#else
				ReadTouches();
			#endif

			ApplyNewStates();
		}

		#region mouse

		private TouchState? _leftMouse;
		private TouchState? _rightMouse;

		void ReadMouse()
		{
			var leftPressed =  Input.GetMouseButton(0);
			var rightPressed = Input.GetMouseButton(1);

			var mousePos = _component.TouchToComponentPoint(Input.mousePosition);

			var leftPos = mousePos;
			var rightPos = leftPos + Vector2.one;

			UpdateMouseTouch(ref _leftMouse, leftPressed, leftPos, 0);
			UpdateMouseTouch(ref _rightMouse, rightPressed, rightPos, 1);

			if (_leftMouse.HasValue)
				_newStates.Add(_leftMouse.Value);

			if (_rightMouse.HasValue)
				_newStates.Add(_rightMouse.Value);
		}

		private void UpdateMouseTouch(ref TouchState? touch, bool isPressed, Vector2 mousePos, int touchId)
		{
			if (isPressed)
			{
				if (touch == null)
				{
					touch = new TouchState(touchId, TouchPhase.Began, mousePos);
				}
				else
				{
					var currentLoc = touch.Value;
					var state = (currentLoc.position == mousePos)
						? TouchPhase.Stationary
						: TouchPhase.Moved;

					touch = new TouchState(currentLoc.id, state, mousePos);
				}
			}
			else
			{
				if (touch != null)
				{
					var currentLoc = touch.Value;
					if (currentLoc.phase != TouchPhase.Ended)
						touch = new TouchState(currentLoc.id, TouchPhase.Ended, mousePos);
					else
						touch = null;
				}
			}
		}

		#endregion

		#region touch

		void ReadTouches()
		{
			var touchCount = Input.touchCount;
			for (int i = 0; i < touchCount; i++)
			{
				var touch = Input.GetTouch(i);
				var position = _component.TouchToComponentPoint(touch.position);
				var state = new TouchState(touch.fingerId, touch.phase, position);
				_newStates.Add(state);
			}
		}

		#endregion

		public List<TouchState> touchStates
		{
			get { return _touchStates; }
		}

	}
}
