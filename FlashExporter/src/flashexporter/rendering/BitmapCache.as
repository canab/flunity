package flashexporter.rendering
{
	public class BitmapCache
	{
		private static var _instance:BitmapCache;

		public static function get instance():BitmapCache
		{
			if (!_instance)
				_instance = new BitmapCache();

			return _instance;
		}

		public static function contains(id:Object):Boolean
		{
			return instance.contains(id);
		}

		public static function getFrames(id:Object):Vector.<BitmapFrame>
		{
			return instance.getFrames(id);
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// instance
		//
		///////////////////////////////////////////////////////////////////////////////////*/


		private var _cache:Object = {};

		public function contains(id:Object):Boolean
		{
			return String(id) in _cache;
		}

		public function getFrames(id:Object):Vector.<BitmapFrame>
		{
			var key:String = String(id);

			var frames:Vector.<BitmapFrame> = _cache[key];
			
			if (!frames)
				frames = _cache[key] = new <BitmapFrame>[];

			return frames;
		}
	}
}