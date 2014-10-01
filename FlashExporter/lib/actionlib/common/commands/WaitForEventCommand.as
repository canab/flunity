package actionlib.common.commands
{
	import actionlib.common.events.EventSender;

	public class WaitForEventCommand extends AsincCommand implements ICancelableCommand
	{
		private var _targetEvent:EventSender = new EventSender(this);

		public function WaitForEventCommand(event:EventSender) 
		{
			_targetEvent = event;
		}
		
		public function cancel():void
		{
			if (_targetEvent.hasListener(onTargetEvent))
				_targetEvent.removeListener(onTargetEvent);
		}
		
		override public function execute():void
		{
			_targetEvent.addListener(onTargetEvent);
		}
		
		private function onTargetEvent():void 
		{
			_targetEvent.removeListener(onTargetEvent);
			dispatchComplete();
		}
	}

}