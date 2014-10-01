package actionlib.common.events
{
	internal class EventData
	{
		public var event:EventSender;
		public var handler:Function;
		
		public function EventData(event:EventSender, handler:Function)
		{
			this.event = event;
			this.handler = handler;
		}
		
		public function subscribe():void 
		{
			event.addListener(handler);
		}
		
		public function unsubscribe():void 
		{
			event.removeListener(handler);
		}
		
		public function equals(data:EventData):Boolean
		{
			return (data.event == event && data.handler == handler);
		}
	}
}
