package actionlib.common.commands
{
	public class AsyncWrapper extends AsincCommand
	{
		private var _func:Function;

		public function AsyncWrapper(func:Function)
		{
			_func = func;
		}

		override public virtual function execute():void
		{
			_func();
			dispatchCompleteAsync();
		}
	}
}
