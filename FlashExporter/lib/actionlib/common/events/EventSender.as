package actionlib.common.events
{
	import actionlib.common.errors.NullPointerError;

	public class EventSender
	{
		private var _sender:Object;
		private var _listeners:Array = [];

		public function EventSender(sender:Object)
		{
			_sender = sender;
		}
		
		public function addListener(listener:Function):void
		{
			if (listener == null)
				throw new NullPointerError();
			else if (hasListener(listener))
				throw new Error("List already contains such listener");
			else
				_listeners.push(listener);
		}
		
		public function removeListener(listener:Function):void
		{
			if (listener == null)
				throw new NullPointerError();
			else if (hasListener(listener))
				_listeners.splice(_listeners.indexOf(listener), 1);
			else
				throw new Error("List doesn't contain such listener");
		}
		
		public function dispatch(argument:* = null):void
		{
			// complexity of this method
			// is result of performance optimization

			if (_listeners.length == 0)
				return;

			var handler:Function;
			var numArgs:int;

			if (_listeners.length == 1)
			{
				handler = _listeners[0];
				numArgs = handler.length;

				if (numArgs == 0)
					handler();
				else if (numArgs == 1)
					handler(_sender);
				else if (numArgs == 2)
					handler(_sender, argument);
				else
					throw new ArgumentError();

				return;
			}

			var	_listenersCopy:Array = _listeners.slice();

			for each (handler in _listenersCopy)
			{
				numArgs = handler.length;

				if (numArgs == 0)
					handler();
				else if (numArgs == 1)
					handler(_sender);
				else if (numArgs == 2)
					handler(_sender, argument);
				else
					throw new ArgumentError();
			}
		}

		public function hasListener(func:Function):Boolean
		{
			return _listeners.indexOf(func) >= 0;
		}

		public function set listener(value:Function):void
		{
			addListener(value);
		}
	}
}