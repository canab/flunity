using System.Collections.Generic;

namespace Flunity.Internal
{
	internal class TouchController
	{
		private class TouchSortComparer : IComparer<TouchListener>
		{
			public int Compare(TouchListener a, TouchListener b)
			{
				return b.target.drawOrder - a.target.drawOrder;
			}
		}

		private static readonly IComparer<TouchListener> _sortComparer = new TouchSortComparer();

		private readonly FlashStage _stage;

		private readonly List<TouchListener> _listeners = new List<TouchListener>(32);
		private readonly List<TouchListener> _pressedListeners = new List<TouchListener>(16);

		private readonly List<TouchState> _justPressedTouches = new List<TouchState>(16);
		private readonly List<TouchState> _justReleasedTouches = new List<TouchState>(16);

		private bool _isIterationPhase = false;
		private int _listenersCount;

		public TouchController(FlashStage stage)
		{
			_stage = stage;
			_stage.input.TouchBegan += touch => _justPressedTouches.Add(touch);
			_stage.input.TouchEnded += touch => _justReleasedTouches.Add(touch);
		}

		internal void AddListener(TouchListener listener)
		{
			listener.isRemoved = false;
			_listeners.Add(listener);
		}

		internal void RemoveListener(TouchListener listener)
		{
			listener.isRemoved = true;
			if (!_isIterationPhase)
				_listeners.Remove(listener);
		}

		internal void DoUpdate()
		{
			_isIterationPhase = true;

			_listenersCount = _listeners.Count;
			_listeners.Sort(_sortComparer);

			CheckJustPressedTouches();
			CheckJustReleasedTouches();
			CheckCanceledTouches();
			Cleanup();

			_isIterationPhase = false;
		}

		private void CheckJustPressedTouches()
		{
			foreach (var touch in _justPressedTouches)
			{
				for (int i = 0; i < _listenersCount; i++)
				{
					var listener = _listeners[i];
					if (listener.HandleTouchBegin(touch))
					{
						if (!_pressedListeners.Contains(listener))
							_pressedListeners.Add(listener);

						break;
					}
				}
			}
		}

		private void CheckJustReleasedTouches()
		{
			foreach (var touch in _justReleasedTouches)
			{
				foreach (var listener in _pressedListeners)
				{
					listener.HandleTouchEnd(touch);
				}
			}
		}

		private void CheckCanceledTouches ()
		{
			foreach (var touch in _stage.input.touchStates)
			{
				foreach (var listener in _pressedListeners)
				{
					listener.HandleTouchCancel(touch);
				}
			}
		}

		private void Cleanup ()
		{
			foreach (var listener in _pressedListeners)
			{
				listener.refreshTouchState();
			}

			_pressedListeners.RemoveAll(it => it.isRemoved || it.touches.Count == 0);
			_listeners.RemoveAll(it => it.isRemoved);
			_justPressedTouches.Clear();
			_justReleasedTouches.Clear();
		}		
	}
}