package actionlib.common.resources
{
	import actionlib.common.commands.AsincMacroCommand;
	import actionlib.common.commands.WaitForEventCommand;

	public class PrepareResourcesCommand extends AsincMacroCommand
	{
		private var _urls:Vector.<String>;
		private var _reference:Object;

		public function PrepareResourcesCommand(urls:Vector.<String>, reference:Object)
		{
			_urls = urls;
			_reference = reference;
		}

		override public function execute():void
		{
			addCommands();
			super.execute();
		}

		private function addCommands():void
		{
			for each (var url:String in _urls)
			{
				var bundle:ResourceBundle = ResourceManager.instance.allocateResource(url, _reference);
				
				if (!bundle.isReady)
					add(new WaitForEventCommand(bundle.readyEvent));
			}
		}
	}
}
