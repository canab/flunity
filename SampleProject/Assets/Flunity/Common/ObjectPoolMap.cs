using System;
using System.Collections.Generic;

namespace Flunity.Common
{
	/// <summary>
	/// Holds several pools for different object types.
	/// </summary>
	public class ObjectPoolMap<K, T> where T : class
	{
		public Func<K, T> objectFactory { get; set; }
		public Action<T> resetAction { get; set; }
		public Action<T> initAction { get; set; }
		
		private readonly Dictionary<K, ObjectPool<T>> _dictionary = new Dictionary<K, ObjectPool<T>>();

		public T GetObject(K key)
		{
			return GetPool(key).GetObject();
		}
		
		public void PutObject(K key, T obj)
		{
			GetPool(key).PutObject(obj);
		}

		private ObjectPool<T> GetPool(K key)
		{
			ObjectPool<T> pool;

			if (!_dictionary.TryGetValue(key, out pool))
			{
				pool = new ObjectPool<T>(() => objectFactory(key))
				{
					initAction = initAction,
					resetAction = resetAction,
				};

				_dictionary[key] = pool;
			}

			return pool;
		}
	}
}