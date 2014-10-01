package actionlib.common.commands
{
	import actionlib.common.events.EventSender;

	public class CommandHistory
	{
		private var _maxUndo:int = 0;
		private var _execList:Array = [];
		private var _undoList:Array = [];

		private var _changeEvent:EventSender = new EventSender(this);
		
		public function CommandHistory(maxUndoLevel:int = 0)
		{
			super();
			_maxUndo = maxUndoLevel;
		}
		
		public function get canUndo():Boolean
		{
			return _execList.length > 0;
		}
		
		public function get canRedo():Boolean
		{
			return _undoList.length > 0;
		}
		
		public function set maxUndo(value:int):void
		{
			_maxUndo = value;
			if (checkMaxUndo())
				_changeEvent.dispatch();
		}
		
		private function checkMaxUndo():Boolean
		{
			if (_maxUndo > 0 && _execList.length > maxUndo)
			{
				_execList.splice(0, _execList.length - _maxUndo);
				return true;
			}
			return false;
		}
		
		public function execute(command:IUndoableCommand):void
		{
			_execList.push(command);
			_undoList = [];
			command.execute();
			checkMaxUndo();
			_changeEvent.dispatch();
		}
		
		public function undo():void
		{
			var command:IUndoableCommand = _execList.pop();
			_undoList.push(command);
			command.undo();
			_changeEvent.dispatch();
		}
	
		public function redo():void
		{
			var command:IUndoableCommand = _undoList.pop();
			_execList.push(command);
			command.execute();
			_changeEvent.dispatch();
		}
		
		public function get changeEvent():EventSender { return _changeEvent; }
		
		public function get maxUndo():int { return _maxUndo; }
	}
}