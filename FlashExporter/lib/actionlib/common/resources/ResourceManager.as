package actionlib.common.resources
{
	import actionlib.common.collections.StringMap;
	import actionlib.common.errors.NullPointerError;
	import actionlib.common.logging.Logger;

	import flash.system.ApplicationDomain;

	public class ResourceManager
	{
		private static var _instance:ResourceManager;

		private static var _logger:Logger = new Logger(ResourceManager);

		/**
		 * garbageCollector is running after each GC_COUNTER_MAX allocated resources
		 */
		private static const GC_COUNTER_MAX:int = 50;

		public static function get instance():ResourceManager
		{
			if (!_instance)
				_instance = new ResourceManager(new PrivateConstructor());

			return _instance;
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// instance
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		private var _manager:LoadingManager = new LoadingManager();
		private var _bundles:StringMap = new StringMap(ResourceBundle);
		private var _gcCounter:int = 0;

		//noinspection JSUnusedLocalSymbols
		public function ResourceManager(param:PrivateConstructor)
		{
			super();
		}

		public function allocateResource(url:String, reference:Object):ResourceBundle
		{
			var bundle:ResourceBundle = _bundles[url];

			if (!bundle)
			{
				bundle = new ResourceBundle(url);
				bundle.load(_manager);
				_bundles[url] = bundle;

				if (++_gcCounter >= GC_COUNTER_MAX)
					collectUnusedResources();
			}

			bundle.addReference(reference);

			return bundle;
		}

		public function createInstance(url:String, className:String):Object
		{
			return getBundle(url).createInstance(className);
		}

		public function getDomain(url:String):ApplicationDomain
		{
			return getBundle(url).domain;
		}

		private function getBundle(url:String):ResourceBundle
		{
			var bundle:ResourceBundle = _bundles[url];

			if (!bundle)
				throw new NullPointerError();

			return bundle;
		}

		public function collectUnusedResources():void
		{
			_logger.info("collectUnusedResources()");

			_gcCounter = 0;

			for each (var url:String in _bundles.getKeys())
			{
				var bundle:ResourceBundle = _bundles[url];
				if (!bundle.hasReferences)
				{
					bundle.dispose();
					_bundles.removeKey(url);
				}
			}
		}

		public function printStats(verbose:Boolean = false):void
		{
			var length:int = 0;
			
			for each (var bundle:ResourceBundle in _bundles)
			{
				if (verbose)
					_logger.info(bundle.getVerboseStats());
				else
					_logger.info(bundle.getStats());

				length++;
			}

			_logger.info("total: " + length);
		}
	}

}

internal class PrivateConstructor {}