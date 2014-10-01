using System;
using System.Collections;
using System.Collections.Generic;

namespace Flunity.Common
{
	/// <summary>
	/// Allows change List<T> during an iteration phase.
	/// Newly added items will be iterated in that phase.
	/// Removed items will not be iterated in taht phase.
	/// </summary>
	public struct MutableListIterator<T> : IEnumerator<T>
	{
		private IList<T> _list;
		private int _currentIndex;
		private int _endIndex;
		private bool _listWasChanged;

		public MutableListIterator(IList<T> list) : this()
		{
			_list = list;
			Reset();
		}

		public bool MoveNext()
		{
			_currentIndex++;
			return _currentIndex < _endIndex;
		}

		public void Reset()
		{
			_currentIndex = -1;
			_endIndex = _list.Count;
			_listWasChanged = false;
		}

		public void Add(T item)
		{
			_list.Add(item);
			_listWasChanged = true;
		}

		public void Remove(T item)
		{
			RemoveAt(_list.IndexOf(item));
		}

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _list.Count)
				throw new IndexOutOfRangeException();

			_list.RemoveAt(index);

			_endIndex--;
			
			if (index <= _currentIndex)
				_currentIndex--;

			_listWasChanged = true;
		}

		public void Dispose()
		{
		}

		public T Current
		{
			get
			{
				if (_currentIndex >= 0 && _currentIndex < _endIndex)
					return _list[_currentIndex];
				else
					throw new IndexOutOfRangeException();
			}
		}

		object IEnumerator.Current
		{
			get { return Current; }
		}

		public bool listWasChanged
		{
			get { return _listWasChanged; }
		}
	}
}
