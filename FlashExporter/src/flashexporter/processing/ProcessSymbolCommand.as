package flashexporter.processing
{
	import actionlib.common.commands.IAsincCommand;
	import actionlib.common.commands.ICancelableCommand;
	import actionlib.common.query.conditions.nameIs;
	import actionlib.common.query.from;

	import flashexporter.commands.*;
	import flashexporter.data.Swf;
	import flashexporter.data.Swf;
	import flashexporter.data.Symbol;
	import flashexporter.font.FontProcessor;
	import flashexporter.sprite.SpriteProcessor;
	import flashexporter.timeline.TimeLineProcessor;

	public class ProcessSymbolCommand extends ProcessingCommandBase
	{
		private static function getProcessor(file:Swf, symbol:Symbol, instance:Object):IAsincCommand
		{
			var processor:IAsincCommand;

			if (symbol.isClass)
				processor = TimeLineProcessor.create(file, symbol, instance);
			else if (symbol.isFont)
				processor = FontProcessor.create(symbol, instance);
			else
				processor = SpriteProcessor.create(symbol, instance);

			return processor;
		}

		private var _file:Swf;
		private var _symbol:Symbol;
		private var _command:IAsincCommand;

		public function ProcessSymbolCommand(symbol:Symbol)
		{
			_symbol = symbol;
		}

		override protected function onExecute():void
		{
			_file = appData.getFileByBundleName(_symbol.bundleName);

			if (!_file.isLoaded)
				loadSwf();
			else
				processSymbol();
		}

		private function loadSwf():void
		{
			_command = new LoadSwfCommand(_file);
			_command.completeEvent.addListener(processSymbol);
			_command.execute();
		}

		private function processSymbol():void
		{
			var instance:Object = _file.createInstance(_symbol.id);
			_command = getProcessor(_file, _symbol, instance);

			if (_command)
			{
				_command.completeEvent.addListener(dispatchComplete);
				_command.execute();
			}
			else
			{
				fail("Processor not found: " + _symbol.id);
			}
		}

		override protected function onCancel():void
		{
			if (_command && _command is ICancelableCommand)
			{
				ICancelableCommand(_command).cancel();
				_command = null;
			}
		}
	}
}
