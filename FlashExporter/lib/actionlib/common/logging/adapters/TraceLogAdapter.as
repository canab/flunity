package actionlib.common.logging.adapters
{
	import actionlib.common.logging.ILogAdapter;
	import actionlib.common.logging.LogLevel;

	public class TraceLogAdapter implements ILogAdapter
	{
		public function print(sender:Object, level:LogLevel, message:String):void
		{
			trace(message);
		}
	}
}
