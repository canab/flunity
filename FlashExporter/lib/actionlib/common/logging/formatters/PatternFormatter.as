package actionlib.common.logging.formatters
{
	import actionlib.common.logging.ILogFormatter;
	import actionlib.common.logging.LogLevel;

	public class PatternFormatter implements ILogFormatter
	{
		public static const DEFAULT_PATTERN:String = "{level}: [{sender}] - {message}";

		private var _pattern:String;

		public function PatternFormatter(pattern:String = null)
		{
			_pattern = pattern || DEFAULT_PATTERN;
		}

		public function format(sender:Object, level:LogLevel, message:String):String
		{
			var levelText:String = (level.name.length == 4 ? " " : "") + level.name;

			var senderName:String = String(sender)
					.replace(/\[object (.+)]$/, "$1")
					.replace(/\[class (.+)]$/, "$1");

			return _pattern
					.replace("{level}", levelText)
					.replace("{sender}", senderName)
					.replace("{message}", message);
		}

		public function get pattern():String
		{
			return _pattern;
		}

		public function set pattern(value:String):void
		{
			_pattern = value;
		}
	}
}
