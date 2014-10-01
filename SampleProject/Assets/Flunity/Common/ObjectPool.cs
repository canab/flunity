using System;

namespace Flunity.Common
{
	/// <summary>
	/// Base class for generic pools.
	/// </summary>
	public class ObjectPool
	{
		/// <summary>
		/// Allows to turn off all objects pools.
		/// 
		/// Reusing of objects is a bug-prone approach.
		/// Preventing any object to be reused is helpful do investigate
		/// into some strange bugs an memory leaks.
		/// </summary>
		public static bool globalEnabled = true;
	}

	/// <summary>
	/// Generic object pool.
	/// </summary>
	public class ObjectPool<T> where T : class
	{
		private const int INITIAL_SIZE = 32;

		public bool enabled = true;

		private Func<T> _constructor;
		private Action<T> _resetAction;
		private Action<T> _initAction;

		private T[] _objects;
		private int _objectsCount = 0;
		private int _getCount = 0;
		private int _putCount = 0;

		public ObjectPool(Func<T> constructor)
		{
			_constructor = constructor;
		}
		
		public ObjectPool(Func<T> constructor, int instanceCount)
			:this(constructor)
		{
			PrecacheObjects(instanceCount);
		}
		
		public T GetObject()
		{
			T result;

			if (_objectsCount > 0)
			{
				_objectsCount--;
				
				result = _objects[_objectsCount];
				
				_objects[_objectsCount] = null;
			}
			else
			{
				result = _constructor();
			}

			if (_initAction != null)
				_initAction(result);

			_getCount++;

			return result;
		}

		public void PutObject(T obj)
		{
			if (!enabled || !ObjectPool.globalEnabled)
				return;

			if (_objects == null)
				_objects = new T[INITIAL_SIZE];
			
			_putCount++;
			
			if (_objects.Length == _objectsCount)
				Array.Resize(ref _objects, 2 * objectsCount);

			_objects[_objectsCount] = obj;
			_objectsCount++;
			
			if (_resetAction != null)
				_resetAction(obj);
		}

		/// <summary>
		/// Instantiates specified count of objects into the pool.
		/// </summary>
		public void PrecacheObjects(int countToPut)
		{
			var newSize = _objectsCount + countToPut;
			
			if (_objects == null)
				_objects = new T[newSize];
			else if (_objects.Length < newSize)
				Array.Resize(ref _objects, newSize);

			for (int i = 0; i < countToPut; i++)
			{
				PutObject(_constructor());
			}
		}

		/// <summary>
		/// Action used for instantiating new objects.
		/// </summary>
		public Func<T> constructor
		{
			get { return _constructor; }
			set { _constructor = value; }
		}

		/// <summary>
		/// Called on object when it is being put into the pool
		/// </summary>
		public Action<T> resetAction
		{
			get { return _resetAction; }
			set { _resetAction = value; }
		}

		/// <summary>
		/// Called on object when it is being acquired from the pool
		/// </summary>
		public Action<T> initAction
		{
			get { return _initAction; }
			set { _initAction = value; }
		}

		public String GetStats()
		{
			return String.Format("size: {0}, get: {1}, put: {2}", _objectsCount, _getCount, _putCount);
		}

		public int objectsCount
		{
			get { return _objectsCount; }
		}
	}
}
