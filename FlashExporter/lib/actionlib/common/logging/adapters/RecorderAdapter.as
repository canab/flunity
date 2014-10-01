package actionlib.common.logging.adapters
{
	import actionlib.common.logging.ILogAdapter;
	import actionlib.common.logging.LogLevel;

	public class RecorderAdapter implements ILogAdapter
	{
		private var _nextAdapter:ILogAdapter;
		private var _records:Array = [];
		private var _active:Boolean = false;

		public function RecorderAdapter(nextAdapter:ILogAdapter)
		{
			_nextAdapter = nextAdapter;
		}

		public function print(sender:Object, level:LogLevel, message:String):void
		{
			if (_active)
				_records.push(message);

			_nextAdapter.print(sender,  level, message);
		}

		public function clear():void
		{
			_records = [];
		}

		public function getRecordedString():String
		{
			return _records.join("\n");
		}

		public function get active():Boolean
		{
			return _active;
		}

		public function set active(value:Boolean):void
		{
			_active = value;
		}
	}
}
