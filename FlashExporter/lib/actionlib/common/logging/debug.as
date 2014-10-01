package actionlib.common.logging
{
	public function debug(...args):void
	{
		Logger.debug.apply(null, args);
	}
}
