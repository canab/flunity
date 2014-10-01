using System;
using System.Collections.Generic;
using Flunity.Common;

namespace Flunity.Common
{
	/// <summary>
	/// EventSender for actions without arguments.
	/// </summary>
	public class EventSender : EventSenderBase<Action>
	{
		/// <summary>
		/// Notifies all listeners.
		/// </summary>
		public void Dispatch()
		{
			isDispatchingPhase = true;
			isPropagationTerminated = false;
			iterator = new MutableListIterator<Action>(listeners);

			while (!isPropagationTerminated && iterator.MoveNext())
			{
				iterator.Current.Invoke();
			}

			isDispatchingPhase = false;
		}
	}
	
	/// <summary>
	/// EventSender for actions with one argument.
	/// </summary>
	public class EventSender<T> : EventSenderBase<Action<T>>
	{
		/// <summary>
		/// Notifies all listeners with specified parameter.
		/// </summary>
		public void Dispatch(T param)
		{
			isDispatchingPhase = true;
			isPropagationTerminated = false;
			iterator = new MutableListIterator<Action<T>>(listeners);

			while (!isPropagationTerminated && iterator.MoveNext())
			{
				iterator.Current.Invoke(param);
			}

			isDispatchingPhase = false;
		}
	}

	/// <summary>
	/// EventSender for actions with two arguments.
	/// </summary>
	public class EventSender<T1, T2> : EventSenderBase<Action<T1, T2>>
	{
		/// <summary>
		/// Notifies all listeners with specified parameters.
		/// </summary>
		public void Dispatch(T1 param1, T2 param2)
		{
			isDispatchingPhase = true;
			isPropagationTerminated = false;
			iterator = new MutableListIterator<Action<T1, T2>>(listeners);

			while (!isPropagationTerminated && iterator.MoveNext())
			{
				iterator.Current.Invoke(param1, param2);
			}

			isDispatchingPhase = false;
		}
	}

	/// <summary>
	/// Base class for events subscribing/dispatching.
	/// 
	/// It is safe to add/remove listeners during dispatching phase.
	/// </summary>
	public abstract class EventSenderBase<DelegateType> where DelegateType : class
	{
		protected List<DelegateType> listeners = new List<DelegateType>();
		protected MutableListIterator<DelegateType> iterator;
		protected bool isDispatchingPhase = false;
		protected bool isPropagationTerminated;

		/// <summary>
		/// Adds event listener.
		/// 
		/// It is safe to add listener if it is already added.
		/// Listener will not be added in this case.
		/// 
		/// It is safe to add listener during dispatching phase.
		/// (Newly added listeners will be nonified in the same phase)
		/// </summary>
		/// <param name="listener">Can be null</param>
		public void AddListener(DelegateType listener)
		{
			if (listener == null)
				return;
			
			if (listeners.Contains(listener))
				return;
			
			if (isDispatchingPhase)
				iterator.Add(listener);
			else
				listeners.Add(listener);
		}

		/// <summary>
		/// Removes event listener.
		/// 
		/// It is safe to remove listener if it hasn't been added before.
		/// 
		/// It is safe to remove listener during dispatching phase.
		/// (Removed listeners will not be notified)
		/// </summary>
		/// <param name="listener">Can be null. </param>
		public void RemoveListener(DelegateType listener)
		{
			if (listener == null)
				return;
			
			var index = listeners.IndexOf(listener);

			if (index < 0)
				return;

			if (isDispatchingPhase)
				iterator.RemoveAt(index);
			else
				listeners.RemoveAt(index);
		}

		/// <summary>
		/// Removes all listeners.
		/// It is safe to call this method during dispatching.
		/// </summary>
		public void ClearListeners()
		{
			while (listeners.Count > 0)
			{
				RemoveListener(listeners[0]);
			}
		}

		/// <summary>
		/// Can be called during dispatching phase to terminate farther propagation.
		/// </summary>
		public void StopPropagation()
		{
			isPropagationTerminated = true;
		}

		public bool hasListeners
		{
			get { return listeners.Count > 0; }
		}
	}

}

