package actionlib.common.commands
{
	public class CallFunctionCommand implements ICommand
	{
		private var _func:Function;
		private var _args:Array;
		private var _thisObject:Object;
		
		public function CallFunctionCommand(func:Function, args:Array = null, thisObject:Object = null)
		{
			_func = func;
			_args = args;
			_thisObject = thisObject;
		}
		
		public function execute():void
		{
			_func.apply(_thisObject, _args);
		}
	}
}