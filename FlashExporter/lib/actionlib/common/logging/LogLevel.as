package actionlib.common.logging
{
	import actionlib.common.errors.EnumInstantiationError;
	import actionlib.common.errors.ItemNotFoundError;

	public class LogLevel
	{
		private static var _enumMap:Object = {};
		private static var _constructed:Boolean = false;

		public static const DEBUG:LogLevel = addEnum("debug", 0);
		public static const INFO:LogLevel = addEnum("info", 1);
		public static const WARN:LogLevel = addEnum("warn", 2);
		public static const ERROR:LogLevel = addEnum("error", 3);
		public static const NONE:LogLevel = addEnum("none", 4);

		private static function addEnum(name:String, order:int):LogLevel
		{
			return _enumMap[name] = new LogLevel(name, order);
		}

		public static function getLevel(name:String):LogLevel
		{
			var level:LogLevel = _enumMap[name];

			if (!level == -1)
				throw new ItemNotFoundError();

			return level;
		}


		//-- instance --//


		private var _name:String;
		private var _order:int;

		public function LogLevel(name:String, order:int)
		{
			if (_constructed)
				throw new EnumInstantiationError();

			_name = name;
			_order = order;
		}

		public function get name():String
		{
			return _name;
		}

		public function get order():int
		{
			return _order;
		}

		public function toString():String
		{
			return "LogLevel[" + _name + "]";
		}
	}
}
