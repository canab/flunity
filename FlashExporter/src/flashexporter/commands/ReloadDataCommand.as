package flashexporter.commands
{
	import actionlib.common.commands.CommandQueue;
	import actionlib.common.commands.ICancelableCommand;

	import flash.filesystem.File;

	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Swf;

	import shared.air.utils.FileUtil;
	import shared.air.utils.FolderScanner;

	public class ReloadDataCommand extends AsyncAppCommand implements ICancelableCommand
	{
		private var _queue:CommandQueue = new CommandQueue();

		override public virtual function execute():void
		{
			app.view.clearLog();
			appData.files = readFiles();

			for each (var file:Swf in appData.files)
			{
				if (file.isChanged)
					_queue.add(new LoadSwfCommand(file));
			}

			_queue.onComplete(onLoadComplete)
					.execute();
		}

		private function readFiles():Vector.<Swf>
		{
			var files:Vector.<File> = new FolderScanner(app.flashDir)
					.where(FileUtil.extensionIs("swf"))
					.findFiles();

			var result:Vector.<Swf> = new <Swf>[];
			for each (var file:File in files)
			{
				var existingSwf:Swf = appData.getSwfByPath(file.nativePath);
				result.push(existingSwf || new Swf(file));
			}

			return result;
		}

		private function onLoadComplete():void
		{
			appData.symbols = appData.getAllSymbols(appData.files);
			dispatchComplete();
		}

		public function cancel():void
		{
			_queue.cancel();
		}
	}
}
