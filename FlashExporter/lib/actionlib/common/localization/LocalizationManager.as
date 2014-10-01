package actionlib.common.localization
{
	import actionlib.common.interfaces.IConverter;

	public class LocalizationManager
	{
		private static var _instance:LocalizationManager;

		private var _locale:String;
		private var _bundles:Object = { };
		private var _urlPattern:String = "resources/localization/{locale}/{bundleId}.xml";
		private var _messageConverter:IConverter = new DefaultMessageConverter();

		//noinspection JSUnusedLocalSymbols
		public function LocalizationManager(param:PrivateConstructor)
		{
			super();
		}

		public static function get instance():LocalizationManager
		{
			if (!_instance)
				_instance = new LocalizationManager(new PrivateConstructor());

			return _instance;
		}

		public function addBundle(bundleId:String):MessageBundle
		{
			if (hasBundle(bundleId))
				throw new Error("Bundle '" + bundleId + "' already exists.");

			var bundle:MessageBundle = new MessageBundle(bundleId);
			new LoadMessagesCommand(bundle).execute();

			return _bundles[bundleId] = bundle;
		}

		public function removeBundle(bundleId:String):void
		{
			if (hasBundle(bundleId))
				delete _bundles[bundleId];
			else
				throw new Error("Bundle '" + bundleId + "' does not exist.");
		}

		public function getBundle(bundleId:String):MessageBundle
		{
			var bundle:MessageBundle = _bundles[bundleId];

			if (!bundle)
				throw new Error("Bundle '" + bundleId + "' does not exist.");

			return bundle;
		}

		public function hasBundle(bundleId:String):Boolean
		{
			return (bundleId in _bundles);
		}

		private function updateBundles():void
		{
			for each (var bundle:MessageBundle in _bundles)
			{
				new LoadMessagesCommand(bundle).execute();
			}
		}

		internal function getUrl(bundleId:String):String
		{
			return _urlPattern
					.replace("{locale}", locale)
					.replace("{bundleId}", bundleId);
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		public function get locale():String
		{
			return _locale;
		}

		public function set locale(value:String):void
		{
			if (_locale != value)
			{
				_locale = value;
				updateBundles();
			}
		}

		public function get urlPattern():String
		{
			return _urlPattern;
		}

		public function set urlPattern(value:String):void
		{
			_urlPattern = value;
		}

		public function get messageConverter():IConverter
		{
			return _messageConverter;
		}

		public function set messageConverter(value:IConverter):void
		{
			_messageConverter = value;
		}
	}

}

internal class PrivateConstructor
{
}