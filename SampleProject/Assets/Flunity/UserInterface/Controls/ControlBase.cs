#region using

using System;
using System.Collections.Generic;
using Flunity.UserInterface.Layouts;
using UnityEngine;

#endregion

namespace Flunity.UserInterface.Controls
{
	public class ControlBase : DisplayContainer
	{
		#region nested

		private class ControlDepthComparer : IComparer<ControlBase>
		{
			public int Compare(ControlBase a, ControlBase b)
			{
				return b.GetDepth().CompareTo(a.GetDepth());
			}
		}

		#endregion

		#region static

		private static readonly ControlDepthComparer _depthComparer = new ControlDepthComparer();
		private static readonly List<ControlBase> _validationList = new List<ControlBase>(32);
		private static FlashStage _currentStage;

		public static void SetCurrentStage(FlashStage stage)
		{
			if(_currentStage != null)
			{
				_currentStage.updateEvent.RemoveListener(validateAllControls);
			}

			_currentStage = stage;
			_currentStage.updateEvent.AddListener(validateAllControls);
		}

		private static void AddToValidationList(ControlBase control)
		{
			if (!control._isScheduledForValidation)
			{
				control._isScheduledForValidation = true;
				_validationList.Add(control);
			}
		}

		public static void validateAllControls()
		{
			_validationList.Sort(_depthComparer);

			/** List can be modified this in iteration*/
			// Analysis disable once ForCanBeConvertedToForeach
			for (int i = 0; i < _validationList.Count; i++)
			{
				var control = _validationList[i];
				control._isScheduledForValidation = false;
				control.ValidateControl();
			}
			
			_validationList.Clear();
			
		}

		#endregion

		public Vector2 measuredSize { get; private set; }

		public float disabledAlpha = 0.5f;

		private bool _isLayoutValid = true;
		private bool _isLayoutSuspended = false;
		private bool _isScheduledForValidation = false;

		private ILayout _layout;
		private List<Anchor> _anchors;
		private Vector2 _size = Vector2.zero;
		private Vector2 _minSize = Vector2.zero;
		private bool _autoSize = false;
		private DisplayObject _controlBackground;

		public ControlBase() {}

		public ControlBase(DisplayContainer parent) : base(parent){}

		protected internal override void OnChildrenChanged()
		{
			InvalidateLayout();
		}

		protected override void OnEnabledChange()
		{
			alpha = isTouchEnabled ? 1.0f : disabledAlpha;
		}

		public void AddAnchor(DisplayObject source, AnchorProperty sourceProp,
			DisplayObject target, AnchorProperty targetProp,
			float multiplier = 1, bool roundToInt = true)
		
		{
			if (_anchors == null)
				_anchors = new List<Anchor>(4);

			_anchors.Add(new Anchor(source, sourceProp, target, targetProp, multiplier, roundToInt));
		}

		public void InvalidateLayout()
		{
			if (_isLayoutValid && !_isLayoutSuspended)
			{
				_isLayoutValid = false;
				AddToValidationList(this);
			}
		}

		public void ValidateControl()
		{
			if (_isLayoutValid)
				return;

			measuredSize = MeasureSize();

			if (_autoSize)
			{
				_isLayoutSuspended = true;
				size = measuredSize;
				_isLayoutSuspended = false;
			}

			ApplyLayout();
			
			ApplyAnchors();
			
			_isLayoutValid = true;
		}

		protected void ApplyAnchors()
		{
			if (_anchors != null)
			{
				foreach (var a in _anchors)
				{
					a.apply();
				}
			}
		}

		protected virtual Vector2 MeasureSize()
		{
			return _layout != null ? _layout.measureSize(this) : Vector2.one;
		}

		protected virtual void ApplyLayout()
		{
			if (_controlBackground != null)
				_controlBackground.DetachFromParent();
			
			if (_layout != null)
				_layout.apply(this);
			
			if (_controlBackground != null)
			{
				AddChildAt(_controlBackground, 0);
				_controlBackground.position = Vector2.zero;
				_controlBackground.size = size;
			}
		}

		public override Vector2 size
		{
			get { return _size; }
			set
			{
				_size.x = Math.Max(value.x, _minSize.x);
				_size.y = Math.Max(value.y, _minSize.y);

				InvalidateLayout();
			}
		}

		public virtual ILayout layout
		{
			get { return _layout; }
			set
			{
				if (_layout != value)
				{
					_layout = value;
					InvalidateLayout();
				}
			}
		}

		public bool autoSize
		{
			get { return _autoSize; }
			set
			{
				if (_autoSize != value)
				{
					_autoSize = value;
					InvalidateLayout();
				}
			}
		}

		public Vector2 minSize
		{
			get { return _minSize; }
			set
			{
				if (_minSize != value)
				{
					_minSize = value;
					
					if (_minSize.x > _size.x || _minSize.y > _size.y)
						size = minSize;
				}
			}
		}

		public float minWidth
		{
			get { return minSize.x; }
			set { minSize = new Vector2(value, minSize.y); }
		}

		public float minHeight
		{
			get { return minSize.y; }
			set { minSize = new Vector2(minSize.y, value); }
		}
		
		public DisplayObject controlBackground
		{
			get { return _controlBackground; }
			set
			{ 
				if (_controlBackground != null)
					_controlBackground.DetachFromParent();
				
				_controlBackground = value;
				
				InvalidateLayout();
			}
		}
	}
}