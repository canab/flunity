package actionlib.common.commands
{
	import actionlib.common.errors.NullPointerError;

	public class AsincMacroCommand extends AsincCommand implements ICancelableCommand
	{
		private var _commands:Vector.<IAsincCommand> = new <IAsincCommand>[];
		private var _completedCommands:Vector.<IAsincCommand> = new <IAsincCommand>[];

		private var _started:Boolean = false;
		private var _canceled:Boolean = false;
		private var _completed:Boolean = false;

		public function AsincMacroCommand()
		{
			super()
		}

		public function add(command:IAsincCommand):AsincMacroCommand
		{
			if (!command)
				throw new NullPointerError();

			_commands.push(command);

			return this;
		}

		public function addAll(commands:/*iterable*/Object):AsincMacroCommand
		{
			for each (var command:IAsincCommand in commands)
			{
				add(command);
			}

			return this;
		}

		override public function execute():void
		{
			if (_started)
				throw new Error("Command is already executed.");

			_started = true;

			if (_commands.length == 0)
				add(new CallLaterCommand());

			for each (var command:IAsincCommand in _commands)
			{
				command.completeEvent.addListener(onCommandComplete);
				command.execute();
			}
		}

		private function onCommandComplete(command:IAsincCommand):void
		{
			command.completeEvent.removeListener(onCommandComplete);

			_completedCommands.push(command);

			if (_completedCommands.length == _commands.length)
				complete();
		}

		private function complete():void
		{
			_started = false;
			_completed = true;
			dispatchComplete();
		}

		public function cancel():void
		{
			for each (var command:IAsincCommand in _commands)
			{
				if (isCommandCompleted(command))
					continue;

				command.completeEvent.removeListener(onCommandComplete);

				if (command is ICancelableCommand)
					ICancelableCommand(command).cancel();
			}

			_started = false;
			_canceled = true;
		}

		public function isCommandCompleted(command:IAsincCommand):Boolean
		{
			return _completedCommands.indexOf(command) >= 0;
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		 //
		 // get/set
		 //
		 ///////////////////////////////////////////////////////////////////////////////////*/

		public function get commands():Vector.<IAsincCommand>
		{
			return _commands;
		}

		public function get started():Boolean
		{
			return _started;
		}

		public function get canceled():Boolean
		{
			return _canceled;
		}

		public function get completed():Boolean
		{
			return _completed;
		}
	}
}