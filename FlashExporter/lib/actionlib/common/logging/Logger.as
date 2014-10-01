package actionlib.common.logging
{
	import actionlib.common.collections.WeakObjectMap;
	import actionlib.common.logging.adapters.TraceLogAdapter;
	import actionlib.common.logging.formatters.PatternFormatter;
	import actionlib.common.utils.ReflectUtil;

	public class Logger
	{
		internal static var loggers:WeakObjectMap = new WeakObjectMap(Logger);

		private static var _defaultAdapter:ILogAdapter;
		private static var _defaultFormatter:ILogFormatter;
		private static var _defaultLevel:LogLevel = LogLevel.DEBUG;
		private static var _defaultLogger:Logger;
		private static var _config:LoggerConfig = new LoggerConfig();

		public static function debug(...args):void
		{
			defaultLogger.debug.apply(null, args);
		}

		public static function info(...args):void
		{
			defaultLogger.info.apply(null, args);
		}

		public static function warn(...args):void
		{
			defaultLogger.warn.apply(null, args);
		}

		public static function error(...args):void
		{
			defaultLogger.debug.apply(null, args);
		}

		public static function setProperties(properties:Object):void
		{
			_config.setProperties(properties);
		}

		/**
		 * Set level for the given logger or group of loggers
		 * @param logger
		 * can be full class name as String or Class
		 * @param level
		 */
		public static function setLevel(logger:*, level:LogLevel):void
		{
			_config.setLevel(logger, level);
		}

		public static function get defaultAdapter():ILogAdapter
		{
			if (!_defaultAdapter)
				_defaultAdapter = new TraceLogAdapter();

			return _defaultAdapter;
		}

		public static function set defaultAdapter(value:ILogAdapter):void
		{
			if (!value)
				throw new Error("logAdapter cannot be null.");

			_defaultAdapter = value;
		}

		public static function get defaultFormatter():ILogFormatter
		{
			if (!_defaultFormatter)
				_defaultFormatter = new PatternFormatter();

			return _defaultFormatter;
		}

		public static function set defaultFormatter(value:ILogFormatter):void
		{
			_defaultFormatter = value;
		}

		public static function get defaultLogger():Logger
		{
			if (!_defaultLogger)
				_defaultLogger = new Logger("Logger");

			return _defaultLogger;
		}

		public static function set defaultLogger(value:Logger):void
		{
			_defaultLogger = value;
		}

		public static function get defaultLevel():LogLevel
		{
			return _defaultLevel;
		}

		public static function set defaultLevel(value:LogLevel):void
		{
			_defaultLevel = value;
		}


		//-- instance --//


		private var _name:String;
		private var _sender:Object;
		private var _adapter:ILogAdapter;
		private var _formatter:ILogFormatter;
		private var _level:LogLevel = defaultLevel;

		public function Logger(sender:Object)
		{
			_sender = sender;

			_name = (sender is String)
					? String(sender)
					: ReflectUtil.getFullName(sender);

			_level = _config.getLevel(_name);

			loggers[this] = true;
		}

		public function debug(...args):void
		{
			if (_level.order <= LogLevel.DEBUG.order)
				print(LogLevel.DEBUG, joinArgs(args));
		}

		public function info(...args):void
		{
			if (_level.order <= LogLevel.INFO.order)
				print(LogLevel.INFO, joinArgs(args));
		}

		public function warn(...args):void
		{
			if (_level.order <= LogLevel.WARN.order)
				print(LogLevel.WARN, joinArgs(args));
		}

		public function error(...args):void
		{
			if (_level.order <= LogLevel.ERROR.order)
				print(LogLevel.ERROR, joinArgs(args));
		}

		private function joinArgs(args:Array):String
		{
			return args.join(" ");
		}

		protected function print(level:LogLevel, message:String):void
		{
			var text:String = formatter.format(_sender, level, message);
			adapter.print(_sender, level, text);
		}


		//-- get/set --//


		public function get adapter():ILogAdapter
		{
			if (!_adapter)
				_adapter = defaultAdapter;

			return _adapter;
		}

		public function set adapter(value:ILogAdapter):void
		{
			_adapter = value;
		}

		public function get formatter():ILogFormatter
		{
			if (!_formatter)
				_formatter = defaultFormatter;

			return _formatter;
		}

		public function set formatter(value:ILogFormatter):void
		{
			_formatter = value;
		}

		public function get level():LogLevel
		{
			return _level;
		}

		public function set level(value:LogLevel):void
		{
			_level = value;
		}

		public function get name():String
		{
			return _name;
		}
	}
}