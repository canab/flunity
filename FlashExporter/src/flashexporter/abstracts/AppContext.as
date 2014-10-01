package flashexporter.abstracts
{
	import flash.utils.getTimer;

	import flashexporter.data.AppData;
	import flashexporter.ToolApplication;
	import flashexporter.data.AppData;

	public class AppContext
	{
		private static var _time:int;

		public static function get app():ToolApplication
		{
			return ToolApplication.instance;
		}

		public static function get appData():AppData
		{
			return ToolApplication.instance.appData;
		}

		public static function measureTime():void
		{
			_time = getTimer();
		}

		public static function traceTime(caption:String):void
		{
			var seconds:Number = (getTimer() - _time) / 1000.0;
			app.logger.debug(caption + ": " + seconds + " sec.");
		}

	}
}
