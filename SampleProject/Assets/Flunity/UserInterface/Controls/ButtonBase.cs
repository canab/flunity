using System;
using Flunity.Utils;
using UnityEngine;

namespace Flunity.UserInterface.Controls
{
	public abstract class ButtonBase : ControlBase, IButton
	{
		public event Action<ButtonBase> pressEvent;
		public event Action<ButtonBase> releaseEvent;
		public event Action<ButtonBase> leaveEvent;
		
		public Action<ButtonBase> clickAction;
		public Rect? customHitArea = null;

		private bool _isPressed = false;
		private bool _active = true;

		protected ButtonBase(DisplayContainer parent) : base(parent) {}

		public void OnButtonPress()
		{
			isPressed = true;

			SetPressedState();

			pressEvent.Dispatch(this);
		}
		
		public void OnButtonRelease()
		{
			isPressed = false;

			SetReleasedState();
			
			releaseEvent.Dispatch(this);
			clickAction.Dispatch(this);
		}

		public void OnButtonLeave()
		{
			isPressed = false;

			SetReleasedState();
			
			leaveEvent.Dispatch(this);
		}

		public virtual Rect hitArea
		{
			get
			{
				return customHitArea != null
					? customHitArea.Value
					: size.ToRect();
			}
		}

		public bool isPressed
		{
			get
			{
				return _isPressed;
			}
			set
			{
				if(_isPressed == value)
					return;

				_isPressed = value;
				
				if (_isPressed)
				{
					SetPressedState();
				}
				else
				{
					SetReleasedState();
				}
			}
		}

		public bool active 
		{
			get { return _active; }
			set 
			{
				if(_active == value)
					return;

				_active = value;
				if(_isPressed)
				{
					_isPressed = false;
					SetReleasedState();
				}

				if(_active)
				{
					SetActivatedState();
				}
				else
				{
					SetDeactivatedState();
				}
			}
		}

		protected abstract void SetPressedState();
		
		protected abstract void SetReleasedState();

		protected abstract void SetActivatedState();

		protected abstract void SetDeactivatedState();
	}
}