package actionlib.common.ui
{
	import actionlib.common.collections.ObjectMap;
	import actionlib.common.display.StageReference;
	import actionlib.common.events.EventSender;

	import flash.display.Stage;
	import flash.events.Event;
	import flash.events.KeyboardEvent;

	public class KeyboardManager
	{
		private static var _instance:KeyboardManager;

		private static function get instance():KeyboardManager
		{
			if (!_instance)
				_instance = new KeyboardManager(new PrivateConstructor());

			return _instance;
		}

		public static function clearKeys():void
		{
			instance.clearKeys();
		}

		public static function isKeyPressed(keyCode:int):Boolean
		{
			return instance.isKeyPressed(keyCode);
		}

		public static function get pressEvent():EventSender
		{
			return instance.pressEvent;
		}

		public static function get releaseEvent():EventSender
		{
			return instance.releaseEvent;
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		 //
		 // instance
		 //
		 ///////////////////////////////////////////////////////////////////////////////////*/

		private var _pressEvent:EventSender = new EventSender(this);
		private var _releaseEvent:EventSender = new EventSender(this);
		private var _pressedKeys:ObjectMap = new ObjectMap(int, Boolean);

		//noinspection JSUnusedLocalSymbols
		public function KeyboardManager(param:PrivateConstructor)
		{
			addStageListeners();
		}

		private function addStageListeners(e:Event = null):void
		{
			var stage:Stage = StageReference.stage;
			stage.addEventListener(KeyboardEvent.KEY_DOWN, onKeyDown);
			stage.addEventListener(KeyboardEvent.KEY_UP, onKeyUp);
			stage.addEventListener(Event.DEACTIVATE, onDeactivate);
		}

		private function onDeactivate(event:Event):void
		{
			clearKeys();
		}

		public function clearKeys():void
		{
			_pressedKeys = new ObjectMap();
		}

		private function onKeyDown(e:KeyboardEvent):void
		{
			_pressedKeys[e.keyCode] = true;
			_pressEvent.dispatch(e);
		}

		private function onKeyUp(e:KeyboardEvent):void
		{
			_pressedKeys.removeKey(e.keyCode);
			_releaseEvent.dispatch(e);
		}

		public function isKeyPressed(keyCode:int):Boolean
		{
			return _pressedKeys.containsKey(keyCode);
		}

		public function get pressedKeys():Array
		{
			var result:Array = [];
			for (var keyCode:Object in _pressedKeys)
			{
				result.push(keyCode);
			}

			return result;
		}

		public function get pressEvent():EventSender { return _pressEvent; }

		public function get releaseEvent():EventSender { return _releaseEvent; }

	}
}

internal class PrivateConstructor
{
}