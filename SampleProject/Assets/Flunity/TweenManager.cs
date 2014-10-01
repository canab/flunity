using System;
using System.Collections.Generic;
using Flunity.Common;
using Flunity.Easing;
using UnityEngine;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Controller, calls update methon on all active tweens.
	/// </summary>
	public class TweenManager
	{
		#region pool

		internal static ObjectPool<List<Tweener>> listPool = new ObjectPool<List<Tweener>>(
			() => new List<Tweener>(tweensPerTarget))
		{
			resetAction = list => list.Clear(),
		};

		public static void InitPool(int totalTweens, int tweensPerTarget = 4, int propsPerTween = 4)
		{
			TweenManager.propsPerTween = propsPerTween;
			TweenManager.tweensPerTarget = tweensPerTarget;
			TweenManager.totalTweens = totalTweens;

			Tweener.pool.PrecacheObjects(totalTweens);
			TweenDataHolder.pool.PrecacheObjects(totalTweens);
			listPool.PrecacheObjects(totalTweens);
		}

		#endregion

		#region static

		internal static int totalTweens = 32;
		internal static int tweensPerTarget = 4;
		internal static int propsPerTween = 4;

		private static readonly Dictionary<object, List<Tweener>> _targetsTweenMap
			= new Dictionary<object, List<Tweener>>(totalTweens);

		private static UnityEventDispatcher _unityEventDispatcher;

		internal static UnityEventDispatcher unityEventDispatcher
		{
			get
			{
				if (_unityEventDispatcher == null)
				{
					var gameObject = new GameObject();
					gameObject.hideFlags = HideFlags.HideInHierarchy;
					UnityEngine.Object.DontDestroyOnLoad(gameObject);
					_unityEventDispatcher = gameObject.AddComponent<UnityEventDispatcher>();
				}

				return _unityEventDispatcher;
			}
		}

		private static TweenManager _instance;


		internal static TweenManager instance
		{
			get
			{
				if (_instance == null)
					_instance = new TweenManager();

				return _instance;
			}
		}

		public static bool HasAnyTweensOf(object target)
		{
			List<Tweener> tweeners;

			_targetsTweenMap.TryGetValue(target, out tweeners);

			if (tweeners == null)
				return false;

			foreach (var tweener in tweeners)
			{
				if (!tweener.isRemoved)
					return true;
			}

			return false;
		}
		
		public static void RemoveAllTweensOf(object target)
		{
			List<Tweener> tweeners;

			_targetsTweenMap.TryGetValue(target, out tweeners);

			if (tweeners == null)
				return;

			for (int i = tweeners.Count - 1; i >= 0; i--)
			{
				var tweener = tweeners[i];
				tweener.manager.RemoveSafe(tweener);
			}
		}


		#endregion 

		private int _defaultDuration = 500;
		private EasyFunction _defaultEasing = Quad.easeOut;

		private Tweener _head = null;
		
		private int _tweensCount = 0;
		private bool _isDispatcherActive = false;
		private bool _paused = false;
		private Action _processingDelegate;
		private Tweener _currentTween;
		private Tweener _nextTweener;

		public TweenManager()
		{
			_processingDelegate = ProcessTweens;
		}

		#region public

		/// <summary>
		/// Creates new Tweener
		/// </summary>
		public Tweener Tween(object target)
		{
			if (target == null)
				throw new ArgumentNullException();

			var tweener = Tweener.pool.GetObject();
			tweener.Initialize(this, target);
			AddTween(tweener);
			UpdateDispatcher();
			return tweener;
		}

		/// <summary>
		/// Creates new Tweener
		/// </summary>
		public Tweener Tween<TTarget, TValue>(
			TTarget target,
			int duration,
			ITweenProperty<TValue> property,
			TValue to,
			EasyFunction easing = null) where TTarget : class
		{
			var tweener = Tween(target);

			if (duration >= 0)
				tweener.Duration(duration);

			if (easing != null)
				tweener.Easing(easing);
			
			tweener.Add(property, to);

			return tweener;
		}
		
		public void PauseAllTweens()
		{
			if (!_paused)
			{
				_paused = true;
				UpdateDispatcher();
			}
		}

		public void ResumeAllTweens()
		{
			if (_paused)
			{
				_paused = false;
				UpdateDispatcher();
			}
		}

		public void RemoveAllTweens()
		{
			var tweener = _head;
			while (tweener != null)
			{
				var nextTweener = tweener.next;
				RemoveSafe(tweener);
				tweener = nextTweener;
			}
		}

		/// <summary>
		/// Disposes all tweens of specified object
		/// </summary>
		public void RemoveTweensOf(object target)
		{
			List<Tweener> tweeners;
			
			_targetsTweenMap.TryGetValue(target, out tweeners);

			if (tweeners == null)
				return;

			for (int i = tweeners.Count - 1; i >= 0; i--)
			{
				if (tweeners[i].manager == this)
					RemoveSafe(tweeners[i]);
			}
		}

		private void RemoveSafe(Tweener tweener)
		{
			if (tweener.isRemoved)
				return;

			if (tweener != _currentTween && tweener != _nextTweener)
				RemoveTween(tweener);
			else
				tweener.isRemoved = true;
		}

		public int defaultDuration
		{
			get { return _defaultDuration; }
			set { _defaultDuration = value; }
		}

		public EasyFunction defaultEasing
		{
			get { return _defaultEasing; }
			set { _defaultEasing = value; }
		}

		public int tweensCount
		{
			get { return _tweensCount; }
		}

		#endregion

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// private
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		#region add/remove

		private void AddTween(Tweener tweener)
		{
			_tweensCount++;
			InsertIntoList(tweener);
			AddToMap(tweener);
		}

		private void InsertIntoList(Tweener tweener)
		{
			tweener.next = _head;
			tweener.prev = null;

			if (_head != null)
				_head.prev = tweener;

			_head = tweener;
		}

		private void AddToMap(Tweener tweener)
		{
			List<Tweener> tweens;
			_targetsTweenMap.TryGetValue(tweener.target, out tweens);

			if (tweens == null)
				tweens = _targetsTweenMap[tweener.target] = listPool.GetObject();

			tweens.Add(tweener);
		}

		private void RemoveTween(Tweener tweener)
		{
			_tweensCount--;

			DeleteFromList(tweener);
			RemoveFromMap(tweener);

			var target = tweener;
			while (target != null)
			{
				var chain = target.chain;
				Tweener.pool.PutObject(target);
				target = chain;
			}
		}

		private void DeleteFromList(Tweener tweener)
		{
			var prevTweener = tweener.prev;
			var nextTweener = tweener.next;

			tweener.prev = tweener.next = null;

			if (prevTweener != null)
				prevTweener.next = nextTweener;

			if (nextTweener != null)
				nextTweener.prev = prevTweener;

			if (tweener == _head)
				_head = nextTweener;
		}

		private void RemoveFromMap(Tweener tweener)
		{
			List<Tweener> tweens;
			var exists = _targetsTweenMap.TryGetValue(tweener.target, out tweens);

			if (!exists)
				return;

			tweens.Remove(tweener);

			if (tweens.Count == 0)
			{
				listPool.PutObject(tweens);
				_targetsTweenMap.Remove(tweener.target);
			}
		}

		#endregion

		#region processing

		private void ProcessTweens()
		{
			var timeStep = TimingUtil.FramesToTime(1);

			_currentTween = _head;

			while (_currentTween != null)
			{
				_nextTweener = _currentTween.next;

				if (_currentTween.isRemoved)
				{
					RemoveTween(_currentTween);
				}
				else
				{
					if (!_currentTween.isActive)
					{
						_currentTween.Activate();
						OverrideProperties(_currentTween);
					}

					_currentTween.DoStep(timeStep);

					if (_currentTween.isRemoved)
						RemoveTween(_currentTween);
					else if (_currentTween.isCompleted)
						FinishTween(_currentTween);
				}

				_currentTween = _nextTweener;
			}

			UpdateDispatcher();
		}

		private void OverrideProperties(Tweener sourceTweener)
		{
			var targetTweeners = _targetsTweenMap[sourceTweener.target];
			var propsToOverride = sourceTweener.properties;

			foreach (var targetTweener in targetTweeners)
			{
				if (targetTweener == sourceTweener)
					continue;

				if (targetTweener.isRemoved)
					continue;

				OverrideChain(targetTweener, propsToOverride);
			}
		}

		private void OverrideChain(Tweener tweener, TweenPropertyMap propsToOverride)
		{
			while (tweener != null)
			{
				tweener.OverrideProperties(propsToOverride);
				tweener = tweener.chain;
			}
		}

		private void FinishTween(Tweener tweener)
		{
			if (tweener.chain != null)
			{
				AddTween(tweener.chain);
				tweener.chain = null;
			}

			RemoveTween(tweener);
		}

		#endregion

		private void UpdateDispatcher()
		{
			if (_isDispatcherActive && (_paused || _head == null))
			{
				_isDispatcherActive = false;
				unityEventDispatcher.onFixedUpdate.RemoveListener(_processingDelegate);
			}
			else if (!_isDispatcherActive && !_paused && _head != null)
			{
				_isDispatcherActive = true;
				unityEventDispatcher.onFixedUpdate.AddListener(_processingDelegate);
			}
		}

		/**
		 * Low performance!
		 * @return multiline text
		 */
		public String GetDebugInfo()
		{
			var dictSize = _targetsTweenMap.Count;

			var listSize = 0;
			for (var tweener = _head; tweener != null; tweener = tweener.next)
			{
				listSize++;
			}

			var text = ""
			           + "tweeners: " + _tweensCount + "\n"
			           + "dictSize: " + dictSize + "\n"
			           + "listSize: " + listSize + "\n"
			           + "active  : " + _isDispatcherActive;

			return text;
		}
	}
}