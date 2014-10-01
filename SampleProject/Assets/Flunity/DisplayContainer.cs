using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flunity.Utils;

namespace Flunity
{
	/// <summary>
	/// Container for DisplayObjects.
	/// All children inherit matrix and color transformation from parent container.
	/// </summary>
	public class DisplayContainer : DisplayObject, IEnumerable<DisplayObject>
	{
		#region static

		public static DisplayContainer CreateFrom(params DisplayObject[] children)
		{
			var container = new DisplayContainer();
			foreach (var displayObject in children)
			{
				container.AddChild(displayObject);
			}
			return container;
		}

		#endregion

		public Rect? predefinedBounds = null;
		
		private LinkedList<DisplayObject> _children = new LinkedList<DisplayObject>();

		public DisplayContainer()
		{
		}
		
		public DisplayContainer(DisplayContainer parent)
		{
			this.parent = parent;
		}

		internal override void InternalAddedToStage(FlashStage stage)
		{
			base.InternalAddedToStage(stage);
			foreach (var child in _children)
			{
				child.stage = stage;
			}
		}

		internal override void InternalRemovedFromStage(FlashStage stage)
		{
			base.InternalRemovedFromStage(stage);
			foreach (var child in _children)
			{
				child.stage = null;
			}
		}

        /// <summary>
        /// Adds specified children to this <c>DisplayContainer</c>.
        /// </summary>
        /// <param name="children">Children to add</param>
		public void AddChildren(IEnumerable<DisplayObject> children)
		{
			foreach (var child in children)
			{
				AddChild(child);
			}
		}
		
        /// <summary>
        /// Adds specified child to this <c>DisplayContainer</c> 
        /// </summary>
        /// <param name="child">Child to add.</param>
		public void AddChild(DisplayObject child)
		{
			AddChildAt(child, numChildren);
		}
		
        /// <summary>
        /// Adds child at the specified position.
        /// </summary>
        /// <param name="child">DisplayObject to add</param>
        /// <param name="position">Child position</param>
		public void AddChildAt(DisplayObject child, Vector2 position)
		{
			child.position = position;
			AddChild(child);
		}

        /// <summary>Adds child object before the target object.</summary>
        /// <param name="before">DisplayObject before which to add child</param>
        /// <param name="child">DisplayObject to add</param>
        /// <exception cref="System.Exception">Thrown when source and target is the same object</exception>
		public void AddChildBefore(DisplayObject before, DisplayObject child)
		{
			AssertNotDrawPhase();

			if (before == child)
				throw new Exception("Source and target is the same object");
			
			if (before.parent != this)
			{
				AddChild(child);
				return;
			}

			MoveFromParent(child);
			_children.AddBefore(before.node, child.node);
			ProcessAdding(child);
		}

        /// <summary>Adds child object after the target object.</summary>
        /// <param name="after">DisplayObject after which to add child</param>
        /// <param name="child">DisplayObject to add</param>
        /// <exception cref="System.Exception">Thrown when source and target is the same object</exception>
		public void AddChildAfter(DisplayObject after, DisplayObject child)
		{
			AssertNotDrawPhase();

			if (after == child)
				throw new Exception("Source and targets are same objects");

			if (after.parent != this)
			{
				AddChild(child);
				return;
			}

			MoveFromParent(child);
			_children.AddAfter(after.node, child.node);
			ProcessAdding(child);
		}

        /// <summary>
        /// Adds specified <c>DisplayObject</c> at specified index
        /// </summary>
        /// <param name="child">Child to add</param>
        /// <param name="childNum">Position Index</param>
		public void AddChildAt(DisplayObject child, int childNum)
		{
			AssertNotDrawPhase();
			
			if (child.parent == this)
			{
				SetChildIndex(child, childNum);
				return;
			}

			childNum = childNum.ClampInt(0, _children.Count);
			
			MoveFromParent(child);

			if (childNum == 0)
				_children.AddFirst(child.node);
			else if (childNum == _children.Count)
				_children.AddLast(child.node);
			else
				_children.AddBefore(GetChildAt(childNum).node, child.node);

			ProcessAdding(child);
		}

        /// <summary>
        /// Removes specified <c>DisplayObject</c> from its parent.
        /// </summary>
        /// <param name="child">Child to remove from its parent</param>
		private void MoveFromParent(DisplayObject child)
		{
			if (child.parent != null)
				child.parent.RemoveChild(child);
		}

        /// <summary>
        /// Removes specified child DisplayObject
        /// </summary>
        /// <param name="child">Child to remove</param>
		public void RemoveChild(DisplayObject child)
		{
			AssertNotDrawPhase();

			_children.Remove(child.node);

			child.stage = null;
			child.InternalSetParent(null);
			OnChildrenChanged();
		}

		private void ProcessAdding(DisplayObject child)
		{
			child.InternalSetParent(this);
			child.stage = stage;
			child.colorDirty = true;
			child.transformDirty = true;
			OnChildrenChanged();
		}

		protected internal virtual void OnChildrenChanged()
		{
		}

		public override void Draw()
		{}

        /// <summary>
        /// Returns index of the specified child.
        /// </summary>
        /// <param name="child">Child to get index of</param>
        /// <returns>Index of the specified child.</returns>
		public int GetChildIndex(DisplayObject child)
		{
			if (child.parent != this)
				return -1;

			var index = 0;
			for (var t = _children.First; t.Value != child; t = t.Next)
			{
				index++;
			}

			return index;
		}

		public void SetChildIndex(DisplayObject child, int childIndex)
		{
			AssertNotDrawPhase();
			ValidateExistingChild(child);

			childIndex = childIndex.ClampInt(0, _children.Count - 1);

			_children.Remove(child.node);

			if (childIndex == 0)
				_children.AddFirst(child.node);
			else if (childIndex == _children.Count)
				_children.AddLast(child.node);
			else
				_children.AddBefore(GetChildAt(childIndex).node, child.node);
		}

        /// <summary>
        /// Returns child <c>DisplayObject</c> at the specified index.
        /// </summary>
        /// <param name="childIndex">Index of child to retrieve</param>
        /// <returns>Child <c>DisplayObject</c> at the specified index</returns>
		public DisplayObject GetChildAt(int childIndex)
		{
			if (childIndex < 0 || childIndex >= _children.Count)
				throw new IndexOutOfRangeException("Cannot find child at: " + childIndex + " (numChildren = " + _children.Count);

			var t = _children.First;
			for (var i = 0; i < childIndex; i++)
			{
				t = t.Next;
			}

			return t.Value;
		}

		public void BringToTop(DisplayObject child)
		{
			SetChildIndex(child, _children.Count - 1);
		}

		public void SendToBack(DisplayObject child)
		{
			SetChildIndex(child, 0);
		}

		public override Rect GetInternalBounds()
		{
			return predefinedBounds != null
				? predefinedBounds.Value
				: GetChildrenBounds();
		}

		public Rect GetChildrenBounds()
		{
			var resultBounds = new Rect();
			var firstChild = true;

			foreach (var child in _children)
			{
				if (!child.visible)
					continue;

				var childBounds = child.GetLocalBounds();

				if (firstChild)
				{
					firstChild = false;
					resultBounds = childBounds;
				}
				else
				{
					GeomUtil.UnionRect(ref resultBounds, ref childBounds, out resultBounds);
				}
			}

			return resultBounds;
		}

		public void RemoveChildren()
		{
			while (numChildren > 0)
			{
				RemoveChild(_children.First.Value);
			}
		}

		private void AssertNotDrawPhase()
		{
			if (isOnStage && stage.isDrawPhase)
				throw new Exception("Cannon modify display list in draw phase");
		}

		private void ValidateExistingChild(DisplayObject child)
		{
			if (child.parent != this)
				throw new Exception("Container does not contain this child");
		}

		public int numChildren
		{
			get { return _children.Count; }
		}

		public DisplayObject this[string childName]
		{
			get { return GetChildByName(childName); }
	    }

		public DisplayObject this[int childNum]
		{
			get { return GetChildAt(childNum); }
	    }

	    public DisplayObject GetChildByName(string elementName)
	    {
	        foreach (var child in _children)
	        {
                if (child.name == elementName)
                    return child;
	        }
	        return null;
	    }

		public List<DisplayObject> GetChildren(Predicate<DisplayObject> filter = null)
		{
			return GetChildren<DisplayObject>(filter);
		}

		public List<T> GetChildren<T>(Predicate<T> filter = null) where T : DisplayObject
		{
			var result = new List<T>();
			foreach (var item in _children)
			{
				var child = item as T;
				if (child == null)
					continue;

				if (filter == null || filter(child))
					result.Add(child);
			}
			return result;
		}

		public List<DisplayObject> GetNestedChildren(Predicate<DisplayObject> filter = null)
		{
			return GetNestedChildren<DisplayObject>(filter);
		}

		public List<T> GetNestedChildren<T>(Predicate<T> filter = null) where T : DisplayObject
		{
			var result = new List<T>();
			var iterator = GetTreeIterator();

			while (iterator.MoveNext())
			{
				var child = iterator.Current as T;
				if (child == null)
					continue;

				if (filter == null || filter(child))
					result.Add(child);
			}
			return result;
		}

		public T GetChild<T>() where T : DisplayObject
		{
			var type = typeof(T);
			foreach (var child in _children)
			{
				if (type.IsInstanceOfType(child))
					return (T)child;
			}
			return null;
		}

		protected override void ResetDisplayObject()
		{
			RemoveChildren();
			base.ResetDisplayObject();
		}

		public override void ValidateDisplayObject()
		{
			base.ValidateDisplayObject();
			
			var iterator = GetTreeIterator();
			while (iterator.MoveNext())
			{
				var child = iterator.Current;
				child.UpdateTransform();
				child.transformDirty = false;
			}
		}

		public DisplayTreeIterator GetTreeIterator()
		{
			return new DisplayTreeIterator(this);
		}

		#region enumerable

		public LinkedList<DisplayObject>.Enumerator GetEnumerator()
		{
			return _children.GetEnumerator();
		}
		
		IEnumerator<DisplayObject> IEnumerable<DisplayObject>.GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _children.GetEnumerator();
		}

		#endregion

		public IEnumerable<DisplayObject> children
		{
			get { return _children; }
			set
			{
				RemoveChildren();
				foreach (var displayObject in value)
				{
					AddChild(displayObject);
				}
			}
		}

		public bool HasChild(DisplayObject child)
		{
			return child.parent == this;
		}
	}
}