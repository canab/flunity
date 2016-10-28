using System;
using UnityEngine;
using Flunity.Utils;

namespace Flunity
{
	/// <summary>
	/// Helper methods for DisplayObjects
	/// </summary>
	public static class DisplayUtil
	{
		/// <summary>
		/// Cached delegate: DisplayObject.DetachFromContainer()
		/// </summary>
		public static readonly Action<object> detachObject = it => ((DisplayObject)it).DetachFromParent();

		/// <summary>
		/// Cached delegate: DisplayObject.visible = false
		/// </summary>
		public static readonly Action<object> hideObject = it => ((DisplayObject)it).visible = false;

		/// <summary>
		/// Cached delegate: DisplayObject.visible = true
		/// </summary>
		public static readonly Action<object> showObject = it => ((DisplayObject)it).visible = true;

		/// <summary>
		/// Cached delegate: DisplayObject.DetachFromContainer()
		/// </summary>
		public static readonly Action<DisplayObject> detach = it => it.DetachFromParent();

		/// <summary>
		/// Cached delegate: DisplayObject.visible = false
		/// </summary>
		public static readonly Action<DisplayObject> hide = it => it.visible = false;

		/// <summary>
		/// Cached delegate: DisplayObject.visible = true
		/// </summary>
		public static readonly Action<DisplayObject> show =	it => it.visible = true;

		/// <summary>
		/// Adjusts scale and position to fit target in specified bounds.
		/// </summary>
		/// <param name="target">Target to fit to specified bounds</param>
		/// <param name="bounds">Bounds to fit target into</param>
		public static void FitToBounds(this DisplayObject target, Rect bounds)
		{
			AdjustScale(target, bounds.width, bounds.height);
			ClampBounds(target, bounds);
		}

		/// <summary>
		/// Adjusts target scale to fit the specified size.
		/// </summary>
		public static void AdjustScale(this DisplayObject target, float maxWidth, float maxHeight)
		{
			var bounds = target.GetInternalBounds();

			// Analysis disable CompareOfFloatsByEqualityOperator
			if (bounds.width == 0 || bounds.height == 0)
				return;
			// Analysis restore CompareOfFloatsByEqualityOperator

			var scale = Math.Min(maxWidth / bounds.width, maxHeight / bounds.height);

			target.scaleXY = scale;
		}

		/// <summary>
		/// Adjusts position to ensure that object's bounds aren't outside the specified bounds
		/// </summary>
		public static void ClampBounds(this DisplayObject target, Rect bounds)
		{
			var rect = target.GetLocalBounds();

			if (rect.xMin < bounds.xMin)
				target.x += bounds.xMin - rect.xMin;
			else if (rect.xMax > bounds.xMax)
				target.x += bounds.xMax - rect.xMax;

			if (rect.yMin < bounds.yMin)
				target.y += bounds.yMin - rect.yMin;
			else if (rect.yMax > bounds.yMax)
				target.y += bounds.yMax - rect.yMax;
		}

		/// <summary>
		/// Animates transparancy to 0, then makes target invisible.
		/// If duration is 0, does id immediately.
		/// </summary>
		public static void HideWithAlpha(this DisplayObject target, int duration = 300)
		{
			if (duration > 0)
			{
				target.Tween(duration, DisplayObject.ALPHA, 0)
					.OnComplete(hideObject);
			}
			else
			{
				target.alpha = 0;
				target.visible = false;
			}
		}
		
		/// <summary>
		/// Animates transparancy to 0, then detaches target from parent.
		/// If duration is 0, does id immediately.
		/// </summary>
		public static void RemoveWithAlpha(this DisplayObject target, int duration = 300, int delay = 0)
		{
			if (duration == 0 && delay == 0)
			{
				target.alpha = 0;
				target.DetachFromParent();
			}
			else
			{
				target.Tween(delay)
					.Chain(duration, DisplayObject.ALPHA, 0)
					.OnComplete(detachObject);
			}
		}

		/// <summary>
		/// Makes target visible, then animates transparancy from 0 to 1.
		/// If duration is 0, does it immediately.
		/// </summary>
		public static void ShowWithAlpha(this DisplayObject target, int duration = 300, int delay = 0)
		{
			target.visible = true;

			if (duration == 0 && delay == 0)
			{
				target.alpha = 1;
			}
			else
			{
				target.alpha = 0;
				target.Tween(delay)
					.Chain(duration, DisplayObject.ALPHA, 1);
			}
		}

		/// <summary>
		/// Launches animation on all target's children;
		/// </summary>
		public static void PlayAllChildren(this DisplayContainer target, bool random = false)
		{
			foreach (var displayObject in target)
			{
				if (displayObject.totalFrames > 1 || displayObject is MovieClip)
				{
					if (random)
						displayObject.GotoRandomFrame();
					displayObject.Play();
				}
			}
		}

		/// <summary>
		/// Stops animation on all target's children;
		/// </summary>
		public static void StopAllChildren(this DisplayContainer target)
		{
			foreach (var displayObject in target)
			{
				if (displayObject.totalFrames > 1 || displayObject is MovieClip)
				{
					displayObject.Stop();
				}
			}
		}

		/// <summary>
		/// Makes object to be most top in its container.
		/// </summary>
		public static void BringToTop(this DisplayObject target)
		{
			if (target.parent != null)
				target.parent.BringToTop(target);
		}

		/// <summary>
		/// Makes object to be most bottom in its container.
		/// </summary>
		public static void SendToBack(this DisplayObject target)
		{
			if (target.parent != null)
				target.parent.SendToBack(target);
		}

		/// <summary>
		/// Returns top child or null if container is empty.
		/// </summary>
		public static DisplayObject GetTopChild(this DisplayContainer container)
		{
			return container.numChildren > 0
				? container.GetChildAt(container.numChildren - 1)
				: null;
		}

		/// <summary>
		/// Searches from top for child of type T. Returns null if not found.
		/// </summary>
		public static T GetTopChild<T>(this DisplayContainer container) where T:DisplayObject
		{
			DisplayObject result = null;
			foreach (var item in container)
			{
				if (item is T)
					result = item;
			}
			return result as T;
		}

		/// <summary>
		/// Returns bottom child or null if container is empty.
		/// </summary>
		public static DisplayObject GetBottomChild(this DisplayContainer container)
		{
			return container.numChildren > 0
				? container.GetChildAt(0)
				: null;
		}

		/// <summary>
		/// Searches from bottom for child of type T. Returns null if not found.
		/// </summary>
		public static T GetBottomChild<T>(this DisplayContainer container) where T : DisplayObject
		{
			foreach (var item in container)
			{
				var result = item as T;
				if (result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Aligns target vertically and horisontally inside specified bounds.
		/// </summary>
		public static void Align(this DisplayObject target, Rect bounds, HAlign horizontal, VAlign vertical, bool snapToPixels = false)
		{
			var objectBounds = target.GetLocalBounds();
			var x = objectBounds.x;
			var y = objectBounds.y;

			switch (horizontal)
			{
				case HAlign.LEFT:
					x = bounds.xMin;
					break;
				case HAlign.CENTER:
					x = 0.5f * (bounds.xMin + bounds.xMax - objectBounds.width);
					break;
				case HAlign.RIGHT:
					x = bounds.xMax - objectBounds.width;
					break;
			}

			switch (vertical)
			{
				case VAlign.TOP:
					y = bounds.yMin;
					break;
				case VAlign.MIDDLE:
					y = 0.5f * (bounds.yMin + bounds.yMax - objectBounds.height);
					break;
				case VAlign.BOTTOM:
					y = bounds.yMax - objectBounds.height;
					break;
			}

			target.x += x - objectBounds.xMin;
			target.y += y - objectBounds.yMin;

			if (snapToPixels)
				target.position = target.position.RoundToInt();
		}

		/// <summary>
		/// Aligns target to be in center of specified bounds.
		/// </summary>
		public static void AlignCenter(this DisplayObject target, Rect bounds, bool alignToPixels = false)
		{
			Align(target, bounds, HAlign.CENTER, VAlign.MIDDLE, alignToPixels);
		}	

		/// <summary>
		/// Searches parent of type T. Returns null if not found.
		/// </summary>
		public static T FindParent<T>(this DisplayObject target) where T : class
		{
			while (target != null)
			{
				target = target.parent;
				var result = target as T;
				if (result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Returns how deep is object in a Display Tree
		/// </summary>
		public static int GetDepth(this DisplayObject target)
		{
			var result = 0;
			for (var t = target; t.parent != null; t = t.parent)
			{
				result++;
			}
			return result;
		}

		/// <summary>
		/// Detaches target from its parent (parent can be null).
		/// </summary>
		public static void DetachFromParent(this DisplayObject target)
		{
			if (target.parent != null)
				target.parent.RemoveChild(target);
		}
	}
}
