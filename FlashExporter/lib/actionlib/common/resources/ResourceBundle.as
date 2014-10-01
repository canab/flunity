package actionlib.common.resources
{
	import actionlib.common.collections.StringMap;
	import actionlib.common.collections.WeakObjectMap;
	import actionlib.common.errors.NullPointerError;
	import actionlib.common.events.EventSender;
	import actionlib.common.resources.loaders.SWFLoader;

	import flash.display.Loader;
	import flash.system.ApplicationDomain;

	public class ResourceBundle
	{
		private var _readyEvent:EventSender = new EventSender(this);

		private var _swfLoader:SWFLoader;
		private var _nativeLoader:Loader;
		private var _url:String;
		private var _references:WeakObjectMap = new WeakObjectMap();
		private var _manager:LoadingManager;

		public function ResourceBundle(url:String)
		{
			_url = url;
		}

		public function addReference(reference:Object):void
		{
			if (!reference)
				throw new NullPointerError();

			_references[reference] = true;
		}

		public function load(manager:LoadingManager):void
		{
			_manager = manager;
			_swfLoader = new SWFLoader(_url);
			_swfLoader.completeEvent.addListener(onLoad);
			_manager.addLoader(_swfLoader);
		}

		private function onLoad(swfLoader:SWFLoader):void
		{
			_nativeLoader = swfLoader.nativeLoader;

			if (_swfLoader.successful)
				_readyEvent.dispatch();

			_swfLoader = null;
		}

		public function dispose():void
		{
			if (_swfLoader)
			{
				_manager.removeLoader(_swfLoader);
				_swfLoader = null;
			}
			else if (_nativeLoader)
			{
				_nativeLoader.unloadAndStop();
				_nativeLoader = null;
			}
		}

		public function createInstance(className:String):Object
		{
			var classRef:Class = Class(domain.getDefinition(className));
			return new classRef();
		}

		public function getReferences():Array
		{
			return _references.getKeys();
		}

		public function getStats():String
		{
			return "(total: " + _references.getLength() + ") " + _url;
		}

		public function getVerboseStats():String
		{
			var references:Array = getReferences();
			var refMap:StringMap = new StringMap(int);

			for each (var object:Object in references)
			{
				var objectName:String = String(object);

				if (refMap.containsKey(objectName))
					refMap[objectName] = refMap[objectName] + 1;
				else
					refMap[objectName] = 1;
			}

			var result:String = "(total: " + _references.getLength() + ") " + _url;

			for (var key:String in refMap)
			{
				result += "\n\t(" + refMap[key] + ") " + key;
			}

			return result;
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		public function get isReady():Boolean
		{
			return Boolean(_nativeLoader);
		}

		public function get domain():ApplicationDomain
		{
			return _nativeLoader.contentLoaderInfo.applicationDomain;
		}

		public function get readyEvent():EventSender
		{
			return _readyEvent;
		}

		public function get hasReferences():Boolean
		{
			return !_references.isEmpty();
		}
	}
}