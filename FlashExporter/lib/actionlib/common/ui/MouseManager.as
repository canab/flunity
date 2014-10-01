package actionlib.common.ui
{
	import actionlib.common.collections.WeakObjectMap;
	import actionlib.common.display.StageReference;
	import actionlib.common.errors.AlreadyInitializedError;
	import actionlib.common.errors.NotInitializedError;
	import actionlib.common.utils.DisplayUtil;

	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.InteractiveObject;
	import flash.display.Sprite;
	import flash.display.Stage;
	import flash.events.Event;
	import flash.events.MouseEvent;
	import flash.ui.Mouse;

	public class MouseManager
	{
		static private var _instance:MouseManager;

		private static function get instance():MouseManager
		{
			if (!initialized)
				throw new NotInitializedError();

			return _instance;
		}

		public static function initialize(root:Sprite):void
		{
			if (initialized)
				throw new AlreadyInitializedError();

			_instance = new MouseManager(root);
		}

		public static function get initialized():Boolean
		{
			return Boolean(_instance);
		}

		public static function setPointer(pointer:Object, hideMouse:Boolean = true):void
		{
			instance.setPointer(pointer, hideMouse);
		}

		public static function resetPointer():void
		{
			instance.resetPointer();
		}

		public static function registerObject(target:DisplayObject, pointer:Object, hideMouse:Boolean = true):void
		{
			instance.registerObject(target, pointer, hideMouse);
		}

		public static function unRegisterObject(target:DisplayObject):void
		{
			instance.unRegisterObject(target);
		}


		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// instance
		//
		///////////////////////////////////////////////////////////////////////////////////*/
		
		private var _root:Sprite;
		private var _pointer:DisplayObject; 
		private var _targets:WeakObjectMap = new WeakObjectMap(DisplayObject, PointerInfo);

		public function MouseManager(root:Sprite)
		{
			_root = root;
		}
		
		/**
		 * 
		 * @param	pointer
		 * DisplayObject or Class
		 * @param	hideMouse
		 */
		public function setPointer(pointer:Object, hideMouse:Boolean = true):void
		{
			resetPointer();
			
			if (pointer is DisplayObject)
				_pointer = DisplayObject(pointer);
			else if (pointer is Class)
				_pointer = new (pointer as Class)();
			else
				throw new ArgumentError("Pointer should be Sprite or Class");
				
			if (_pointer is InteractiveObject)
				InteractiveObject(_pointer).mouseEnabled = false;
			
			if (_pointer is DisplayObjectContainer)
				DisplayObjectContainer(_pointer).mouseChildren = false;
			
			_root.addChild(_pointer);
			StageReference.stage.addEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
			
			if (hideMouse)
				Mouse.hide();
			
			updatePointer();
		}
		
		public function registerObject(target:DisplayObject, pointer:Object, hideMouse:Boolean = true):void
		{
			var info:PointerInfo = new PointerInfo(pointer, hideMouse);
			
			addListeners(target);

			var stage:Stage = StageReference.stage;

			if (target.hitTestPoint(stage.mouseX, stage.mouseY, true))
				setPointer(info.pointer, info.hideMouse);
			
			_targets[target] = info;
		}
		
		public function unRegisterObject(target:DisplayObject):void
		{
			removeListeners(target);
			_targets.removeKey(target);
		}

		private function addListeners(target:DisplayObject):void
		{
			target.addEventListener(MouseEvent.MOUSE_OVER, onMouseOver);
			target.addEventListener(MouseEvent.MOUSE_OUT, onMouseOut);
			target.addEventListener(Event.REMOVED_FROM_STAGE, onRemovedFromStage);
		}

		private function removeListeners(target:DisplayObject):void
		{
			target.removeEventListener(MouseEvent.MOUSE_OVER, onMouseOver);
			target.removeEventListener(MouseEvent.MOUSE_OUT, onMouseOut);
			target.removeEventListener(Event.REMOVED_FROM_STAGE, onRemovedFromStage);
		}

		private function onMouseOver(e:MouseEvent):void
		{
			var info:PointerInfo = _targets[e.currentTarget];
			setPointer(info.pointer, info.hideMouse);
		}

		private function onMouseOut(e:MouseEvent):void
		{
			resetPointer();
		}

		private function onRemovedFromStage(event:Event):void
		{
			resetPointer();
		}

		public function resetPointer():void
		{
			if (_pointer)
			{
				DisplayUtil.detachFromDisplay(_pointer);
				StageReference.stage.removeEventListener(MouseEvent.MOUSE_MOVE, onMouseMove);
				_pointer = null;
			}
			
			Mouse.show();
		}
		
		private function onMouseMove(e:MouseEvent):void
		{
			updatePointer();
			e.updateAfterEvent();
		}
		
		private function updatePointer():void
		{
			_pointer.x = _root.mouseX;
			_pointer.y = _root.mouseY;
		}
		
		public function get initialized():Boolean
		{
			return Boolean(_root);
		}

		public function get root():Sprite
		{
			return _root;
		}
	}
}

internal class PointerInfo
{
	public var pointer:Object;
	public var hideMouse:Boolean;
	
	public function PointerInfo(pointer:Object, hideMouse:Boolean)
	{
		this.pointer = pointer;
		this.hideMouse = hideMouse;
	}
}