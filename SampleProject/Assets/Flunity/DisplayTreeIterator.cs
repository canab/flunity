using System;
using System.Collections;
using System.Collections.Generic;

namespace Flunity
{
	/// <summary>
	/// Iterates recursively all children of specified Container
	/// </summary>
	public struct DisplayTreeIterator : IEnumerator<DisplayObject>
	{
		private readonly DisplayContainer _root;
		private DisplayObject _current;

		public DisplayTreeIterator(DisplayContainer root)
		{
			_root = root;
			_current = null;
		}

		public bool MoveNext()
		{
			if (_current == null)
			{
				_current = _root;
				return true;
			}

			var currentContainer = _current as DisplayContainer;
			if (currentContainer != null && currentContainer.numChildren > 0)
			{
				_current = currentContainer.GetChildAt(0);
				return true;
			}

			if (_current.node.Next != null)
			{
				_current = _current.node.Next.Value;
				return true;
			}

			var parent = _current.parent;
			while (parent != null && parent != _root && parent.node.Next == null)
			{
				_current = parent;
				parent = _current.parent;
			}

			_current = parent != null && parent != _root
				? parent.node.Next.Value
				: null;

			return _current != null;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		public DisplayObject Current
		{
			get { return _current; }
		}

		public void Dispose()
		{
		}

		object IEnumerator.Current
		{
			get { return _current; }
		}
	}
}