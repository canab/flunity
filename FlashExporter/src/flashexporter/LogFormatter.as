package flashexporter
{
	import actionlib.common.logging.ILogFormatter;
	import actionlib.common.logging.LogLevel;

	public class LogFormatter implements ILogFormatter
	{
		public function format(sender:Object, level:LogLevel, message:String):String
		{
			return (level == LogLevel.ERROR) ? "ERROR: " + message : message;
		}
	}
}