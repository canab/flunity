package actionlib.common.events
{
	import flash.events.EventDispatcher;

	public class EventManager
	{
		static private const NOT_REGISTERED_ERROR:String = "Event is not registered";
		static private const ALREADY_REGISTERED_ERROR:String = "Event is already registered";
		
		private var _events:Vector.<EventData> = new Vector.<EventData>();
		private var _nativeEvents:Vector.<NativeEventData> = new Vector.<NativeEventData>();
		
		public function EventManager()
		{
			super();
		}
		
		public function registerEvent(event:EventSender, handler:Function):void
		{
			var data:EventData = new EventData(event, handler);
			var index:int = getEventIndex(data);
			
			if (index >= 0)
			{
				throw new Error(ALREADY_REGISTERED_ERROR);
			}
			else
			{
				data.subscribe();
				_events.push(data);
			}
		}
		
		public function unregisterEvent(event:EventSender, handler:Function):void
		{
			var data:EventData = new EventData(event, handler);
			var index:int = getEventIndex(data);
			
			if (index == -1)
			{
				throw new Error(NOT_REGISTERED_ERROR);
			}
			else
			{
				data.unsubscribe();
				_events.splice(index, 1);
			}
		}
		
		private function getEventIndex(data:EventData):int
		{
			var n:int = _events.length;
			for (var i:int = 0; i < n; i++)
			{
				if (_events[i].equals(data))
					return i;
			}
			return -1;
		}
		
		public function registerNativeEvent(object:EventDispatcher,
			type:String, listener:Function, useCapture:Boolean = false):void
		{
			var data:NativeEventData = new NativeEventData(object, type, listener, useCapture);
			var index:int = getNativeEventIndex(data);
			
			if (index >= 0)
			{
				throw new Error(ALREADY_REGISTERED_ERROR);
			}
			else
			{
				data.subscribe();
				_nativeEvents.push(data);
			}
		}
		
		public function unregisterNativeEvent(object:EventDispatcher,
			type:String, listener:Function, useCapture:Boolean = false):void
		{
			var data:NativeEventData = new NativeEventData(object, type, listener, useCapture);
			var index:int = getNativeEventIndex(data);
			
			if (index == -1)
			{
				throw new Error(NOT_REGISTERED_ERROR);
			}
			else
			{
				data.unsubscribe();
				_nativeEvents.splice(index, 1);
			}
		}	

		private function getNativeEventIndex(data:NativeEventData):int
		{
			var n:int = _nativeEvents.length;
			for (var i:int = 0; i < n; i++)
			{
				if(_nativeEvents[i].equals(data))
					return i;
			}
			return -1;
		}
		
		public function clearEvents():void
		{
			for each (var eventData:EventData in _events)
			{
				eventData.unsubscribe();
			}
			
			for each (var nativeEventData:NativeEventData in _nativeEvents)
			{
				nativeEventData.unsubscribe();
			}
			
			_events.length = 0;
			_nativeEvents.length = 0;
		}
	}
	
}
