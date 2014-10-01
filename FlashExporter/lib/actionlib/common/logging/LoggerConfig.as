package actionlib.common.logging
{
	import actionlib.common.collections.StringMap;
	import actionlib.common.utils.ReflectUtil;
	import actionlib.common.utils.StringUtil;

	internal class LoggerConfig
	{
		private var _properties:StringMap = new StringMap(LogLevel);

		public function setProperties(properties:Object):void
		{
			for (var key:String in properties)
			{
				setLevel(key, LogLevel.getLevel(properties[key]));
			}
		}

		public function setLevel(logger:*, level:LogLevel):void
		{
			var loggerName:String = (logger is Class)
					? ReflectUtil.getFullName(logger)
					: String(logger).replace("::", ".");

			overrideRecords(loggerName, level);
			applyLevel(loggerName, level)
		}

		private function overrideRecords(loggerName:String, level:LogLevel):void
		{
			for each (var key:String in _properties.getKeys())
			{
				if (StringUtil.startsWith(key, loggerName))
					_properties.removeKey(key);
			}
			_properties[loggerName] = level;
		}

		private function applyLevel(loggerName:String, level:LogLevel):void
		{
			for (var key:Object in Logger.loggers)
			{
				var logger:Logger = Logger(key);
				if (StringUtil.startsWith(logger.name, loggerName))
					logger.level = level;
			}
		}

		public function getLevel(loggerName:String):LogLevel
		{
			var maxLength:int = 0;
			var level:LogLevel;

			for (var key:String in _properties)
			{
				if (key.length > maxLength && loggerName.indexOf(key) == 0)
				{
					level = _properties[key];
					maxLength = key.length;
				}
			}

			if (!level)
				level = _properties.root;

			if (!level)
				level = Logger.defaultLevel;

			return level;
		}
	}
}
