package actionlib.common.localization
{
	import actionlib.common.commands.ICommand;
	import actionlib.common.resources.loaders.BinaryLoader;

	internal class LoadMessagesCommand implements ICommand
	{
		private var _bundle:MessageBundle;
		private var _loader:BinaryLoader;

		public function LoadMessagesCommand(bundle:MessageBundle)
		{
			_bundle = bundle;
		}

		public function execute():void
		{
			var url:String = LocalizationManager.instance.getUrl(_bundle.id);
			
			_loader = new BinaryLoader(url);
			_loader.completeEvent.addListener(onLoadComplete);
			_loader.execute();
		}

		private function onLoadComplete(loader:BinaryLoader):void
		{
			if (_loader.successful)
				_bundle.messages = LocalizationManager.instance.messageConverter.convert(loader.data);
		}

	}

}