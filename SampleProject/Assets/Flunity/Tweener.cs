using System;
using Flunity.Common;
using Flunity.Internal;
using UnityEngine;
using Flunity.Easing;

namespace Flunity
{
	/// <summary>
	/// Animates one or more properties on the target object. See TweenManager, TweenExt
	/// </summary>
	public class Tweener
	{
		public static readonly ObjectPool<Tweener> pool =
			new ObjectPool<Tweener>(() => new Tweener())
		{
			resetAction = it => it.Reset(),
		};

		internal TweenPropertyMap properties = new TweenPropertyMap(TweenManager.propsPerTween);

		internal Tweener prev;
		internal Tweener next;
		internal Tweener chain;
		internal TweenManager manager;
		internal object target;

		internal bool isActive;
		internal bool isCompleted;
		internal bool isRemoved;

		private float _elapsed;
		private float _duration;

		private Action _startHandler;
		private Action<object> _startParamHandler;

		private Action _updateHandler;
		private Action<object> _updateParamHandler;

		private Action _completeHandler;
		private Action<object> _completeParamHandler;

		private EasyFunction _easing;
		
		private uint _targetVersion;
		private IReusable _targetAsIReusable;
		private IActiveObject _targetAsIActiveObject;

		private Tweener()
		{}

		internal void Initialize(TweenManager manager, object target)
		{
			this.manager = manager;
			this.target = target;
			
			_duration = Mathf.Min(this.manager.defaultDuration, 1);
			_elapsed = 0;

			_targetAsIActiveObject = target as IActiveObject;
			_targetAsIReusable = target as IReusable;
			_targetVersion = (_targetAsIReusable != null) ? _targetAsIReusable.version : 0;

			isActive = false;
			isCompleted = false;
			isRemoved = false;
		}

		private void Reset()
		{
			foreach (var property in properties)
			{
				TweenDataHolder.pool.PutObject(property.Value);
			}

			properties.Clear();

			prev = null;
			next = null;
			chain = null;
			target = null;
			manager = null;

			_startHandler = null;
			_startParamHandler = null;
			_updateHandler = null;
			_updateParamHandler = null;
			_completeHandler = null;
			_completeParamHandler = null;
			_easing = null;
			_targetAsIReusable = null;
			_targetAsIActiveObject = null;
		}

		/// <summary>
		/// Sets the duration of animation in milliseconds
		/// </summary>
		public Tweener Duration(int value)
		{
			_duration = value;
			return this;
		}

		/// <summary>
		/// Sets an easing equation. Flunity.Easing namespace
		/// </summary>
		public Tweener Easing(EasyFunction value)
		{
			_easing = value;
			return this;
		}

		/// <summary>
		/// Callback, invoked on animation is started.
		/// </summary>
		public Tweener OnStart(Action func)
		{
			_startHandler = func;
			return this;
		}
		
		/// <summary>
		/// Callback with argument, invoked when animation is started.
		/// Target object will be passed.
		/// </summary>
		public Tweener OnStart(Action<object> func)
		{
			_startParamHandler = func;
			return this;
		}

		/// <summary>
		/// Callback, invoked on each animation update.
		/// </summary>
		public Tweener OnUpdate(Action func)
		{
			_updateHandler = func;
			return this;
		}
		
		/// <summary>
		/// Callback with argument, invoked on each animation update.
		/// Target object will be passed.
		/// </summary>
		public Tweener OnUpdate(Action<object> func)
		{
			_updateParamHandler = func;
			return this;
		}

		/// <summary>
		/// Callback, invoked when animation is completed.
		/// </summary>
		public Tweener OnComplete(Action func)
		{
			_completeHandler = func;
			return this;
		}
		
		/// <summary>
		/// Callback with argument, invoked when animation is completed.
		/// Target object will be passed.
		/// </summary>
		public Tweener OnComplete(Action<object> func)
		{
			_completeParamHandler = func;
			return this;
		}

		/// <summary>
		/// Add property to animate
		/// </summary>
		public Tweener Add<T>(ITweenProperty<T> property, T endValue)
		{
			if (properties.ContainsKey(property))
				return this;
			
			var tweenData = TweenDataHolder.pool.GetObject();
			property.WriteValue(tweenData.endValue, endValue);
			properties[property] = tweenData;
			return this;
		}

		/// <summary>
		/// Returns another Tweener instance which will be executed after curent tween is ended.
		/// Default duration will be set.
		/// </summary>
		public Tweener Chain()
		{
			chain = pool.GetObject();
			chain.Initialize(manager, target);
			return chain;
		}

		/// <summary>
		/// Returns another Tweener instance which will be executed after curent tween is ended.
		/// Specified duration will be set.
		/// </summary>
		public Tweener Chain(int duration)
		{
			chain = pool.GetObject();
			chain.Initialize(manager, target);
			chain.Duration(duration);
			return chain;
		}

		public Tweener Chain<TValue>(
			int duration,
			ITweenProperty<TValue> property,
			TValue endValue,
			EasyFunction easing = null)
		{
			chain = pool.GetObject();
			chain.Initialize(manager, target);
			chain.Duration(duration);
			chain.Add(property, endValue);
			if (easing != null)
				chain.Easing(easing);
			return chain;
		}

		internal void Activate()
		{
			InitializeProperties();
			
			isActive = true;

			if (_easing == null)
				_easing = manager.defaultEasing;
			
			if (_startHandler != null)
				_startHandler();
			
			if (_startParamHandler != null)
				_startParamHandler(target);
		}

		private void InitializeProperties()
		{
			foreach (var entry in properties)
			{
				var property = entry.Key;
				var data = entry.Value;
				property.GetValue(data.startValue, target);
			}
		}

		internal void DoStep(float timeStep)
		{
			if (_targetAsIActiveObject != null && !_targetAsIActiveObject.isActivityEnabled)
			{
				isRemoved = true;
				return;
			}
			
			if (_targetAsIReusable != null && _targetAsIReusable.version != _targetVersion)
			{
				isRemoved = true;
				return;
			}

			_elapsed += timeStep;

			var timePosition = _elapsed / _duration;

			if (timePosition < 1)
			{
				var easePosition = _easing(timePosition);

				foreach (var entry in properties)
				{
					var property = entry.Key;
					var data = entry.Value;
					property.Interpolate(target, data.startValue, data.endValue, (float) easePosition);
				}
			}
			else
			{
				SetEndValues();
				isCompleted = true;
			}

			if (_updateHandler != null)
				_updateHandler();
			
			if (_updateParamHandler != null)
				_updateParamHandler(target);

			if (isCompleted)
			{
				if (_completeHandler != null)
					_completeHandler();
				
				if (_completeParamHandler != null)
					_completeParamHandler(target);
			}
		}

		internal void OverrideProperties(TweenPropertyMap entriesToRemove)
		{
			foreach (var entry in entriesToRemove)
			{
				var key = entry.Key;
				if (properties.ContainsKey(key))
				{
					var data = properties[key];
					TweenDataHolder.pool.PutObject(data);
					properties.Remove(key);
				}
			}
		}

		private void SetEndValues()
		{
			foreach (var entry in properties)
			{
				var property = entry.Key;
				var data = entry.Value;
				property.SetValue(target, data.endValue);
			}
		}

		internal bool isEmpty
		{
			get { return properties.Count == 0; }
		}
	}
}