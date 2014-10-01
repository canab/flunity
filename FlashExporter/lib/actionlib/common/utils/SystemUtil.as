package actionlib.common.utils
{
	import actionlib.common.logging.Logger;

	import flash.net.LocalConnection;
	import flash.system.System;

	public class SystemUtil
	{
		public static function callGC(enableHack:Boolean = false):void
		{
			Logger.info("GC called");

			System.gc();

			if (enableHack)
			{
				//hack to force gc to collect all objects
				try
				{
					new LocalConnection().connect("aaa");
					new LocalConnection().connect("aaa");
				}
				catch (e:Error)
				{}
			}
		}
	}
}
