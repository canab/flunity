package actionlib.common.utils
{
	import flash.utils.getTimer;

	public class ProfilingUtil
	{
		public static function traceTime(caption:String, func:Function):void
		{
			var time:Number = getTimer();
			func();
			time = (getTimer() - time);
			trace(caption + ": " + time);
		}
	}
}
