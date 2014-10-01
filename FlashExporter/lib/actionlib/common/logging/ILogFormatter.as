package actionlib.common.logging
{
	public interface ILogFormatter
	{
		function format(sender:Object, level:LogLevel, message:String):String;
	}
}
