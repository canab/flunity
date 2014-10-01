package actionlib.common.events
{
	import flash.events.EventDispatcher;

	internal class NativeEventData
	{
		public var object:EventDispatcher;
		public var type:String;
		public var listener:Function;
		public var useCapture:Boolean;
		
		function NativeEventData(object:EventDispatcher, type:String, listener:Function, useCapture:Boolean)
		{
			this.object = object;
			this.type = type;
			this.listener = listener;
			this.useCapture = useCapture;
		}
		
		public function subscribe():void 
		{
			object.addEventListener(type, listener, useCapture);
		}
		
		public function unsubscribe():void 
		{
			object.removeEventListener(type, listener, useCapture);
		}
		
		public function equals(data:NativeEventData):Boolean
		{
			return (data.object == object
				&& data.type == type
				&& data.listener == listener
				&& data.useCapture == useCapture);
		}
	}

}