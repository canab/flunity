package actionlib.common.resources.loaders
{
	import flash.display.DisplayObject;
	import flash.display.Loader;
	import flash.display.LoaderInfo;
	import flash.events.Event;
	import flash.events.IOErrorEvent;
	import flash.net.URLRequest;
	import flash.system.LoaderContext;

	internal class ContentLoaderBase extends LoaderBase
	{
		private var _nativeLoader:Loader;
		private var _loaderContext:LoaderContext;

		public function ContentLoaderBase(url:String, maxAttempts:int = 3)
		{
			super(url, maxAttempts);
		}

		override protected function startLoading():void
		{
			if (!_nativeLoader)
				_nativeLoader = new Loader();

			addListeners();

			_nativeLoader.load(new URLRequest(url), _loaderContext);
		}

		protected function addListeners():void
		{
			_nativeLoader.contentLoaderInfo.addEventListener(Event.COMPLETE, onLoadComplete);
			_nativeLoader.contentLoaderInfo.addEventListener(IOErrorEvent.IO_ERROR, onLoadError);
		}

		override protected function removeListeners():void
		{
			_nativeLoader.contentLoaderInfo.removeEventListener(Event.COMPLETE, onLoadComplete);
			_nativeLoader.contentLoaderInfo.removeEventListener(IOErrorEvent.IO_ERROR, onLoadError);

			_nativeLoader.contentLoaderInfo.addEventListener(IOErrorEvent.IO_ERROR, fakeErrorHandler);
		}

		private function fakeErrorHandler(event:IOErrorEvent):void
		{
			// do nothing, but prevent unhandled exception
		}

		override protected function stopLoading():void
		{
			try
			{
				_nativeLoader.close();
			}
			catch (e:Error)
			{
			}
		}

		private function onLoadComplete(e:Event):void
		{
			processComplete();
		}

		private function onLoadError(e:Event):void
		{
			processFail();
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		override public function get progress():Number
		{
			var info:LoaderInfo = loaderInfo;

			return (info && info.bytesTotal > 0)
				? info.bytesLoaded / info.bytesTotal
				: 0;
		}

		public function get content():DisplayObject
		{
			return _nativeLoader ? _nativeLoader.content : null;
		}

		public function get loaderInfo():LoaderInfo
		{
			return _nativeLoader ? _nativeLoader.contentLoaderInfo : null;
		}

		public function get nativeLoader():Loader
		{
			return _nativeLoader;
		}

		public function set nativeLoader(value:Loader):void
		{
			_nativeLoader = value;
		}

		public function get loaderContext():LoaderContext
		{
			return _loaderContext;
		}

		public function set loaderContext(value:LoaderContext):void
		{
			_loaderContext = value;
		}
	}

}