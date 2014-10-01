package actionlib.common.ui
{
	import actionlib.common.events.EventSender;

	import flash.display.InteractiveObject;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.geom.Rectangle;

	public class DragController
	{
		public var enabled:Boolean = true;
		public var lockHorizontal:Boolean = false;
		public var lockVertical:Boolean = false;

		private var _startEvent:EventSender = new EventSender(this);
		private var _finishEvent:EventSender = new EventSender(this);
		private var _dragEvent:EventSender = new EventSender(this);

		private var _content:InteractiveObject;
		private var _hitArea:InteractiveObject;
		private var _bounds:Rectangle;

		private var _dX:Number;
		private var _dY:Number;

		private var _startX:Number;
		private var _startY:Number;

		private var _positionChanged:Boolean = false;
		private var _isActive:Boolean = false;

		public function DragController(content:InteractiveObject, hitArea:InteractiveObject = null)
		{
			_content = content;
			_hitArea = hitArea || content;

			_hitArea.addEventListener(MouseEvent.MOUSE_DOWN, onMouseDown);

			_startX = content.x;
			_startY = content.y;
		}

		private function onMouseDown(e:MouseEvent):void
		{
			if (enabled)
				startDrag();
		}

		public function startDrag():void
		{
			_startX = _content.x;
			_startY = _content.y;

			_dX = _content.parent.mouseX - _content.x;
			_dY = _content.parent.mouseY - _content.y;

			_hitArea.stage.addEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			_hitArea.stage.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			_hitArea.stage.addEventListener(Event.ENTER_FRAME, onFrame);

			_isActive = true;
			_startEvent.dispatch();
		}

		private function onMouseUp(e:MouseEvent):void
		{
			stopDrag();
		}

		private function stopDrag():void
		{
			_hitArea.stage.removeEventListener(MouseEvent.MOUSE_UP, onMouseUp);
			_hitArea.stage.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			_hitArea.stage.removeEventListener(Event.ENTER_FRAME, onFrame);

			_isActive = false;
			_finishEvent.dispatch();
		}

		public function undo():void
		{
			_content.x = _startX;
			_content.y = _startY;
		}

		private function onFrame(e:Event):void
		{
			if (!_content.stage)
			{
				stopDrag()
			}
			else if (_positionChanged)
			{
				_positionChanged = false;
				_dragEvent.dispatch();
			}
		}

		private function onMouseMove(e:MouseEvent):void
		{
			var oldX:Number = _content.x;
			var oldY:Number = _content.y;

			if (!lockHorizontal)
				_content.x += _content.parent.mouseX - _dX - _content.x;

			if (!lockVertical)
				_content.y += _content.parent.mouseY - _dY - _content.y;

			if (_bounds)
				checkBounds();

			if (_content.x != oldX || _content.y != oldY)
			{
				_positionChanged = true;
				e.updateAfterEvent();
			}
		}

		private function checkBounds():void
		{
			var rect:Rectangle = _content.getBounds(_content.parent);

			if (!lockHorizontal)
			{
				if (rect.left < _bounds.left)
					_content.x += _bounds.left - rect.left;
				else if (rect.right > _bounds.right)
					_content.x += _bounds.right - rect.right;
			}

			if (!lockVertical)
			{
				if (rect.top < _bounds.top)
					_content.y += _bounds.top - rect.top;
				else if (rect.bottom > _bounds.bottom)
					_content.y += _bounds.bottom - rect.bottom;
			}
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		public function get startEvent():EventSender
		{
			return _startEvent;
		}

		public function get finishEvent():EventSender
		{
			return _finishEvent;
		}

		public function get dragEvent():EventSender
		{
			return _dragEvent;
		}

		public function get bounds():Rectangle
		{
			return _bounds;
		}

		public function set bounds(value:Rectangle):void
		{
			_bounds = value;
		}

		public function get content():InteractiveObject
		{
			return _content;
		}

		public function get isActive():Boolean
		{
			return _isActive;
		}
	}
}