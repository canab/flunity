package actionlib.common.resources.loaders
{
	import actionlib.common.events.EventSender;

	import flash.events.Event;
	import flash.system.ApplicationDomain;

	public class SWFLoader extends ContentLoaderBase
	{
		private var _initEvent:EventSender;

		public function SWFLoader(url:String, maxAttempts:int = 3)
		{
			super(url, maxAttempts);
		}

		override protected function addListeners():void
		{
			super.addListeners();
			nativeLoader.contentLoaderInfo.addEventListener(Event.INIT, onInit);
		}

		override protected function removeListeners():void
		{
			super.removeListeners();
			nativeLoader.contentLoaderInfo.removeEventListener(Event.INIT, onInit);
		}

		private function onInit(event:Event):void
		{
			if (_initEvent)
				_initEvent.dispatch();
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		public function get domain():ApplicationDomain
		{
			return nativeLoader.contentLoaderInfo.applicationDomain;
		}

		public function get initEvent():EventSender
		{
			if (!_initEvent)
				_initEvent = new EventSender(this);

			return _initEvent;
		}
	}

}