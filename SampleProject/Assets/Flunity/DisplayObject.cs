using System;
using System.Collections.Generic;
using Flunity.Common;
using Flunity.Properties;
using Flunity.Utils;
using UnityEngine;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Base class for renderable objects
	/// </summary>
	public abstract class DisplayObject : IFrameAnimable, IReusable, IActiveObject
	{
		#region events

		/// <summary>
		/// Dispatched when an object or its parent is attached to stage
		/// </summary>
		public event Action<DisplayObject> AddedToStage;

		/// <summary>
		/// Dispatched when an object or its parent is removed from stage
		/// </summary>
		public event Action<DisplayObject> RemovedFromStage;

		/// <summary>
		/// dispatched when an animation is completed
		/// </summary>
		public event Action<DisplayObject> PlayCompleted;

		#endregion

		#region tween properties

		public static readonly VectorProperty<DisplayObject> POSITION
			= new VectorProperty<DisplayObject>(o => o.position, (o, value) => o.position = value);

		public static readonly FloatProperty<DisplayObject> X
			= new FloatProperty<DisplayObject>(o => o.x, (o, value) => o.x = value);

		public static readonly FloatProperty<DisplayObject> Y
			= new FloatProperty<DisplayObject>(o => o.y, (o, value) => o.y = value);

		public static readonly FloatProperty<DisplayObject> ROTATION
			= new FloatProperty<DisplayObject>(o => o.rotation, (o, value) => o.rotation = value);

		public static readonly VectorProperty<DisplayObject> SCALE
			= new VectorProperty<DisplayObject>(o => o.scale, (o, value) => o.scale = value);
		
		public static readonly FloatProperty<DisplayObject> SCALE_X
			= new FloatProperty<DisplayObject>(o => o.scaleX, (o, value) => o.scaleX = value);
		
		public static readonly FloatProperty<DisplayObject> SCALE_Y
			= new FloatProperty<DisplayObject>(o => o.scaleY, (o, value) => o.scaleY = value);
		
		public static readonly FloatProperty<DisplayObject> SCALE_XY
			= new FloatProperty<DisplayObject>(o => o.scaleXY, (o, value) => o.scaleXY = value);

		public static readonly VectorProperty<DisplayObject> SIZE
			= new VectorProperty<DisplayObject>(o => o.size, (o, value) => o.size = value);

		public static readonly FloatProperty<DisplayObject> WIDTH
			= new FloatProperty<DisplayObject>(o => o.width, (o, value) => o.width = value);
		
		public static readonly FloatProperty<DisplayObject> HEIGHT
			= new FloatProperty<DisplayObject>(o => o.height, (o, value) => o.height = value);

		public static readonly ColorTransformProperty<DisplayObject> COLOR_TRANSFORM
			= new ColorTransformProperty<DisplayObject>(
				o => o.colorTransform, (o, value) => o.colorTransform = value);

		public static readonly ColorProperty<DisplayObject> COLOR
			= new ColorProperty<DisplayObject>(o => o.color, (o, value) => o.color = value);

		public static readonly ColorProperty<DisplayObject> TINT
		= new ColorProperty<DisplayObject>(o => o.tint, (o, value) => o.tint = value);

		public static readonly FloatProperty<DisplayObject> ALPHA
			= new FloatProperty<DisplayObject>(o => o.alpha, (o, value) => o.alpha= value);
		
		public static readonly FloatProperty<DisplayObject> BRIGHTNESS
			= new FloatProperty<DisplayObject>(o => o.brightness, (o, value) => o.brightness = value);

		public static readonly VectorProperty<DisplayObject> ANCHOR
			= new VectorProperty<DisplayObject>(o => o.anchor, (o, value) => o.anchor = value);
		
		public static readonly BooleanProperty<DisplayObject> TOUCH_ENABLED
			= new BooleanProperty<DisplayObject>(o => o.isTouchEnabled, (o, value) => o.isTouchEnabled = value);
		
		public static readonly BooleanProperty<DisplayObject> VISIBLE
			= new BooleanProperty<DisplayObject>(o => o.visible, (o, value) => o.visible = value);
		
		#endregion

		public string name; 

		internal int timelineInstanceId;

		internal LinkedListNode<DisplayObject> node;

		#region initializing

		protected DisplayObject()
		{
			node = new LinkedListNode<DisplayObject>(this);
			
			// Analysis disable once DoNotCallOverridableMethodsInConstructor
			ResetDisplayObject();

			_updateHandler = HandleUpdate;
		}

		protected virtual void ResetDisplayObject()
		{
			_visible = true;
			_animation = null;
			_isColorInherited = true;
			_isTouchEnabled = true;

			name = null;
			drawOptions = null;
			currentFrame = 0;
			timelineInstanceId = -1;

			ResetTransform();

			AddedToStage = null;
			RemovedFromStage = null;
			PlayCompleted = null;
		}

		/// <summary>
		/// Resets color and geometry transformations
		/// </summary>
		public void ResetTransform()
		{
			_position = Vector2.zero;
			_rotation = 0;
			_colorTransform = new ColorTransform(1);
			_scale = Vector2.one;

			anchor = Vector2.zero;
			transformDirty = true;
			colorDirty = true;
		}
		#endregion

		#region adding/removing

		internal virtual void InternalAddedToStage(FlashStage stage)
		{
			drawOrder = 0;
			CheckUpdateSubscribtion(stage);
			AddedToStage.Dispatch(this);
		}

		internal virtual void InternalRemovedFromStage(FlashStage stage)
		{
			CheckUpdateSubscribtion(stage);
			RemovedFromStage.Dispatch(this);
		}

		internal void InternalSetParent(DisplayContainer value)
		{
			if (_parent != value)
			{
				_parent = value;
				if (_parent != null)
					InternalAdded();
				else
					InternalRemoved();
			}
		}

		internal virtual void InternalRemoved()
		{}

		internal virtual void InternalAdded()
		{}

		#endregion



		#region compositeColor

		internal protected ColorTransform compositeColor;
		internal bool colorDirty;

		internal protected virtual void UpdateColor()
		{
			if (_parent != null && _isColorInherited)
				ColorTransform.Compose(ref _parent.compositeColor, ref _colorTransform, out compositeColor);
			else
				compositeColor = colorTransform;
		}
		#endregion

		#region useParentColor

		private bool _isColorInherited;

		/// <summary>
		/// If true, color will not be composed with parent color.
		/// Parent color transform will not make effect on color of this object.
		/// </summary>
		public bool isColorInherited
		{
			get { return _isColorInherited; }
			set
			{
				_isColorInherited = value;
				colorDirty = true;
			}
		}
		#endregion

		#region color properties

		private ColorTransform _colorTransform;

		/// <summary>
		/// Color transformation of this object.
		/// </summary>
		public ColorTransform colorTransform
		{
			get { return _colorTransform; }
			set
			{
				_colorTransform = value;
				colorDirty = true;
			}
		}

		/// <summary>
		/// Alpha part of colorTransform
		/// </summary>
		public float alpha
		{
			get { return _colorTransform.aMult; }
			set
			{
				_colorTransform.aMult = value;
				colorDirty = true;
			}
		}

		/// <summary>
		/// Brightness of colorTransform
		/// </summary>
		public float brightness
		{
			get { return _colorTransform.GetBrightness(); }
			set
			{
				_colorTransform.SetBrightness(value);
				colorDirty = true;
			}
		}

		/// <summary>
		/// Color part of colorTransform (color multiplier)
		/// </summary>
		public Color color
		{
			get { return _colorTransform.GetColor(); }
			set
			{
				_colorTransform.SetColor(ref value);
				colorDirty = true;
			}
		}

		/// <summary>
		/// Tint part of colorTransform (color offset)
		/// </summary>
		public Color tint
		{
			get { return _colorTransform.GetTint(); }
			set
			{
				_colorTransform.SetTint(ref value);
				colorDirty = true;
			}
		}
		#endregion



		#region compositeMatrix

		internal protected Matrix4x4 compositeMatrix;
		internal bool transformDirty;

		private Vector2 _position;
		private Vector2 _scale;
		private float _rotation;

		internal protected virtual void UpdateTransform()
		{
			if (parent == null)
			{
				MatrixUtil.Create2D(ref _scale, _rotation, ref _position, out compositeMatrix);
			}
			else
			{
				Matrix4x4 localMatrix;
				MatrixUtil.Create2D(ref _scale, _rotation, ref _position, out localMatrix);
				MatrixUtil.Multiply(ref localMatrix, ref _parent.compositeMatrix, out compositeMatrix);
			}
		}

		/// <summary>
		/// Validates transformation matrix for this object, all parents and children.
		/// Normally such calculations are performed during draw phase.
		/// </summary>
		public virtual void ValidateDisplayObject()
		{
			ValidateCompositeMatrix();
		}

		private static readonly List<DisplayObject> _tempDisplayList = new List<DisplayObject>(32);

		private void ValidateCompositeMatrix()
		{
			for (var t = this; t != null; t = t.parent)
			{
				_tempDisplayList.Add(t);
			}

			var dirtyFlag = false;

			for (int i = _tempDisplayList.Count - 1; i >= 0; i--)
			{
				var t = _tempDisplayList[i];
				if (dirtyFlag || t.transformDirty)
				{
					dirtyFlag = true;
					t.UpdateTransform();
				}
			}

			_tempDisplayList.Clear();
		}

		#endregion

		#region transformation methods

		/// <summary>
		/// Transforms global point to local
		/// </summary>
		public Vector2 GlobalToLocal(Vector2 position)
		{
			Vector2 result;
			Matrix4x4 invMatrix;
			MatrixUtil.Inverse(ref compositeMatrix, out invMatrix);
			MatrixUtil.TransformPos(ref position, ref invMatrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms global rect to local
		/// </summary>
		public Rect GlobalToLocal(Rect rect)
		{
			Rect result;
			Matrix4x4 invMatrix;
			MatrixUtil.Inverse(ref compositeMatrix, out invMatrix);
			GeomUtil.GetBounds(ref rect, ref invMatrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms local point to global
		/// </summary>
		public Vector2 LocalToGlobal(Vector2 position)
		{
			Vector2 result;
			MatrixUtil.TransformPos(ref position, ref compositeMatrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms local rect to global
		/// </summary>
		public void LocalToGlobal(ref Rect rect, out Rect result)
		{
			GeomUtil.GetBounds(ref rect, ref compositeMatrix, out result);
		}

		/// <summary>
		/// Transforms local rect to global
		/// </summary>
		public Rect LocalToGlobal(Rect rect)
		{
			Rect result;
			GeomUtil.GetBounds(ref rect, ref compositeMatrix, out result);
			return result;
		}

		/// <summary>
		/// Transforms local point to point in coordinate system of targetContainer
		/// </summary>
		public Vector2 LocalToLocal(Vector2 point, DisplayObject targetObject)
		{
			return targetObject.GlobalToLocal(LocalToGlobal(point));
		}

		/// <summary>
		/// Transforms local (0, 0) to point in coordinate system of targetContainer
		/// </summary>
		public Vector2 LocalToLocal(DisplayObject targetContainer)
		{
			return targetContainer.GlobalToLocal(LocalToGlobal(Vector2.zero));
		}

		/// <summary>
		/// Transforms local rect to rect in coordinate system of targetContainer
		/// </summary>
		public Rect LocalToLocal(Rect rect, DisplayObject targetContainer)
		{
			return targetContainer.GlobalToLocal(LocalToGlobal(rect));
		}

		/// <summary>
		/// Returns bounds of this object in the parent container
		/// </summary>
		public virtual Rect GetLocalBounds()
		{
			Rect result;
			Matrix4x4 localMatrix;
			var bounds = GetInternalBounds();
			MatrixUtil.Create2D(ref _scale, _rotation, ref _position, out localMatrix);
			GeomUtil.GetBounds(ref bounds, ref localMatrix, out result);
			return result;
		}

		/// <summary>
		/// Returns global bounds of this object
		/// </summary>
		public virtual Rect GetGlobalBounds()
		{
			Rect result;
			var bounds = GetInternalBounds();
			GeomUtil.GetBounds(ref bounds, ref compositeMatrix, out result);
			return result;
		}
		#endregion

		#region transformation properties

		/// <summary>
		/// Calculates size of this object using GetInternalBounds() and scale.
		/// </summary>
		public virtual Vector2 size
		{
			get
			{
				var b = GetInternalBounds();
				var s = scale;
				return new Vector2(b.width * s.x, b.height * s.y);
			}
			set
			{
				var b = GetInternalBounds();
				// Analysis disable CompareOfFloatsByEqualityOperator
				if (b.width != 0 && b.height != 0)
					scale = new Vector2(value.x / b.width, value.y / b.height);
				// Analysis restore CompareOfFloatsByEqualityOperator
			}
		}

		/// <summary>
		/// x member of the size
		/// </summary>
		public float width
		{
			get { return size.x; }
			set { size = new Vector2(value, size.y); }
		}

		/// <summary>
		/// y member of the size
		/// </summary>
		public float height
		{
			get { return size.y; }
			set { size = new Vector2(size.x, value); }
		}

		/// <summary>
		/// Position of this object
		/// </summary>
		public Vector2 position
		{
			get { return _position; }
			set
			{
				_position = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// x member of the position
		/// </summary>
		public float x
		{
			get { return _position.x; }
			set
			{
				_position.x = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// y member of the position
		/// </summary>
		public float y
		{
			get { return _position.y; }
			set
			{
				_position.y = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// Rotation of this object
		/// </summary>
		public float rotation
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// Scale of this object
		/// </summary>
		public Vector2 scale
		{
			get { return _scale; }
			set
			{
				_scale = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// x member of the scale
		/// </summary>
		public float scaleX
		{
			get { return _scale.x; }
			set
			{
				_scale.x = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// y member of the scale
		/// </summary>
		public float scaleY
		{
			get { return _scale.y; }
			set
			{
				_scale.y = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// Returns average value of the scale.x and scale.y
		/// Sets the same value to the scale.x and scale.y
		/// </summary>
		public float scaleXY
		{
			get { return 0.5f * (_scale.x + _scale.y); }
			set
			{
				_scale.x = _scale.y = value;
				transformDirty = true;
			}
		}

		/// <summary>
		/// Returns position. Setter changes position and size.
		/// </summary>
		public Vector2 topLeft
		{
			get { return position; }
			set
			{
				size += position - value;
				position = value;
			}
		}

		/// <summary>
		/// Returns x. Setter changes x and width;
		/// </summary>
		public float left
		{
			get { return x; }
			set
			{
				width += x - value;
				x = value;
			}
		}

		/// <summary>
		/// Returns y. Setter changes y and height;
		/// </summary>
		public float top
		{
			get { return y; }
			set
			{
				height += y - value;
				y = value;
			}
		}

		/// <summary>
		/// Returns position + 0.5f * size
		/// </summary>
		public Vector2 center
		{
			get { return position + 0.5f * size; }
		}

		/// <summary>
		/// Returns x + 0.5f * width
		/// </summary>
		public float centerX
		{
			get { return x + 0.5f * width; }
		}

		/// <summary>
		/// Returns y + 0.5f * height
		/// </summary>
		public float centerY
		{
			get { return y + 0.5f * height; }
		}

		/// <summary>
		/// Returns position + size. Setter changes size.
		/// </summary>
		public Vector2 bottomRight
		{
			get { return position + size; }
			set { size = value - position; }
		}

		/// <summary>
		/// Returns x + width. Setter changes width.
		/// </summary>
		public float right
		{
			get { return x + width; }
			set { width = value - x; }
		}

		/// <summary>
		/// Returns y + height. Setter changes height.
		/// </summary>
		public float bottom
		{
			get { return y + height; }
			set { height = value - y; }
		}

		/// <summary>
		/// Returns 0.5f * width
		/// </summary>
		public float halfWidth
		{
			get { return 0.5f * width; }
		}

		/// <summary>
		/// Returns 0.5f * height
		/// </summary>
		public float halfHeight
		{
			get { return 0.5f * height; }
		}
		#endregion



		#region visible

		private bool _visible;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Flunity.DisplayObject"/> is visible.
		/// Invisible object does not update any transformations.
		/// </summary>
		public bool visible
		{
			get { return _visible; }
			set
			{
				if (_visible != value)
				{
					_visible = value;
					colorDirty = true;
					transformDirty = true;
				}
			}
		}
		#endregion

		#region enabled

		private bool _isTouchEnabled;

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Flunity.DisplayObject"/> is enabled.
		/// Disabled object does not handle touch events.
		/// </summary>
		public bool isTouchEnabled
		{
						get { return _isTouchEnabled; }
			set
			{
				if (_isTouchEnabled != value)
				{
					_isTouchEnabled = value;
					OnEnabledChange();
				}
			}
		}

		protected virtual void OnEnabledChange()
		{
		}
		#endregion

		#region anchor

		public Vector2 anchor;

		/// <summary>
		/// Sets the anchor point relative to this object's internal bounds.
		/// </summary>
		public void SetAnchorRelative(float rx, float ry)
		{
			const float EPSILON = 1e-6f;
			anchor.x = width / (Math.Abs(scaleX) < EPSILON ? 1 : scaleX) * rx;
			anchor.y = height / (Math.Abs(scaleY) < EPSILON ? 1 : scaleY) * ry;
		}

		/// <summary>
		/// Calls SetAnchorRelative(0.5f, 0.5f);
		/// </summary>
		public void SetAnchorAtCenter()
		{
			SetAnchorRelative(0.5f, 0.5f);
		}
		#endregion

		#region stage

		private FlashStage _stage;

		/// <summary>
		/// Returns FlashStage this object is attached to.
		/// Returns null if this object is not attached.
		/// </summary>
		public FlashStage stage
		{
			get { return _stage; }
			internal set
			{
				if (_stage != value)
				{
					var prevValue = _stage;

					_stage = value;
					
					if (_stage != null)
						InternalAddedToStage(value);
					else
						InternalRemovedFromStage(prevValue);
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="Flunity.DisplayObject"/> is on stage.
		/// </summary>
		public bool isOnStage
		{
			get { return _stage != null; }
		}
		#endregion

		#region parent

		private DisplayContainer _parent;

		/// <summary>
		/// Returns DisplayContainer this object is directly attached to.
		/// Returns null if this object is not attached.
		/// </summary>
		public DisplayContainer parent
		{
			get { return _parent; }
			set
			{
				if (value != null)
					value.AddChild(this);
				else if (_parent != null)
					_parent.RemoveChild(this);
			}
		}

		#endregion

		#region currentFrame

		private int _currentFrame = 0;

		/// <summary>
		/// Gets or sets the current frame.
		/// </summary>
		public int currentFrame
		{
			get { return _currentFrame; }
			set
			{
				if (_currentFrame != value)
				{
					if (value < 0 || value >= _totalFrames)
						throw new IndexOutOfRangeException();

					_currentFrame = value;

					OnFrameChange();
				}
			}
		}

		protected virtual void OnFrameChange()
		{
		}

		#endregion

		#region totalFrames

		private int _totalFrames = 1;

		/// <summary>
		/// Gets the frames count.
		/// </summary>
		public int totalFrames
		{
			get { return _totalFrames; }
			protected set
			{
				if (_totalFrames < 1)
					throw new ArgumentException("totalFrames");

				_totalFrames = value;
			}
		}
		#endregion



		#region IReusable

		public uint version { get; private set; }

		public void Reuse()
		{
			version += 1;
			ResetDisplayObject();
		}

		#endregion

		#region IActiveObject

		public bool isActivityEnabled
		{
			get { return _stage != null; }
		}

		#endregion

		#region IFrameAnimable

		private FrameAnimation _animation;

		/// <summary>
		/// Holds the frame animation of this object.
		/// </summary>
		public FrameAnimation animation
		{
			get
			{
				if (_animation == null)
				{
					_animation = new FrameAnimation(this);
					_animation.completeHandler = DispatchPlayComplete;
					CheckUpdateSubscribtion(_stage);
				}
				return _animation;
			}
		}

		public virtual void DispatchPlayComplete()
		{
			PlayCompleted.Dispatch(this);
		}

		public DisplayObject OnPlayComplete(Action<DisplayObject> handler)
		{
			PlayCompleted += handler;
			return this;
		}

		#endregion

		#region EnterFrame

		private EventSender<DisplayObject> _enterFrameEvent;

		/// <summary>
		/// Dispatched each frame if the object is on the stage
		/// </summary>
		public event Action<DisplayObject> EnterFrame
		{
			add
			{
				if (_enterFrameEvent == null)
					_enterFrameEvent = new EventSender<DisplayObject>();

				_enterFrameEvent.AddListener(value);

				CheckUpdateSubscribtion(_stage);
			}
			remove
			{
				if (_enterFrameEvent != null)
					_enterFrameEvent.RemoveListener(value);

				CheckUpdateSubscribtion(_stage);
			}
		}

		#endregion

		#region update handler

		private Action _updateHandler;
		private bool _updateSubscribed;

		private void CheckUpdateSubscribtion(FlashStage stage)
		{
			var enterFrameExists = _enterFrameEvent != null	&& _enterFrameEvent.hasListeners;
			var animationExists = animation != null;
			var needUpdate = isOnStage && (enterFrameExists || animationExists);

			if (needUpdate && !_updateSubscribed)
			{
				stage.updateEvent.AddListener(_updateHandler);
				_updateSubscribed = true;
			}
			else if (!needUpdate && _updateSubscribed)
			{
				stage.updateEvent.RemoveListener(_updateHandler);
				_updateSubscribed = false;
			}
		}

		private void HandleUpdate()
		{
			if (_enterFrameEvent != null)
				_enterFrameEvent.Dispatch(this);

			if (_animation != null && _animation.isActive)
				_animation.DoStep();
		}

		#endregion


		#region drawing

		public DrawOptions drawOptions;

		/// <summary>
		/// If set, all graphics which does not intersect drawRect will not be drawn
		/// </summary>
		public Rect? drawRect;

		internal int drawOrder;

		public abstract void Draw();

		/// <summary>
		/// Returns bounds of this object in the internal coordinates system.
		/// </summary>
		public virtual Rect GetInternalBounds()
		{
			return new Rect();
		}

		#endregion

		#region ToString

		public override string ToString()
		{
			return name != null
				? string.Format("{0}[{1}]", GetType().Name, name)
				: GetType().Name;
		}

		#endregion
	}
}