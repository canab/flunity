package actionlib.common.logging
{
	public interface ILogAdapter
	{
		function print(sender:Object, level:LogLevel, message:String):void;
	}
}
