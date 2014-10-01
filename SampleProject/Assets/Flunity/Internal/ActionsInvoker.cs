using System;
using System.Collections.Generic;

namespace Flunity.Internal
{
	/// <summary>
	/// Holds actions for invoking them later by calling InvokeActions().
	/// Thread safe.
	/// </summary>
	public class ActionsInvoker
	{
		private readonly object _lock = new Object();
 		private readonly List<Action> _actions = new List<Action>();
 		private readonly List<Action> _addedActions = new List<Action>();
		private bool _isIterationPhase = false;

		private volatile bool _hasActions = false;

		/// <summary>
		/// Adds an action to the queue.
		/// </summary>
		/// <description>
		/// It is safe to add actions during "invoking" phase.
		/// The same action will not be only once.
		/// </description>
		/// <param name="action">Action to add, can be null</param>
		public void AddAction(Action action)
		{
			if (action == null)
				return;

			lock (_lock)
			{
				if (_isIterationPhase)
				{
					if (!_addedActions.Contains(action))
						_addedActions.Add(action);
				}
				else
				{
					if (!_actions.Contains(action))
						_actions.Add(action);
				}

				_hasActions = true;
			}
		}

		/// <summary>
		/// Invokes sceduled actions.
		/// </summary>
		public void InvokeActions()
		{
			if (!_hasActions)
				return;

			lock (_lock)
			{
				_isIterationPhase = true;
				
				foreach (var action in _actions)
				{
					action.Invoke();
				}

				_actions.Clear();

				if (_addedActions.Count > 0)
				{
					_actions.AddRange(_addedActions);
					_addedActions.Clear();
				}

				_hasActions = _actions.Count > 0;
				_isIterationPhase = false;
			}
		}

		public bool hasActions
		{
			get { return _hasActions; }
		}
	}
}