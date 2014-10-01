package flashexporter.processing
{
	import actionlib.common.commands.AsyncWrapper;
	import actionlib.common.commands.CommandQueue;
	import actionlib.common.query.from;

	import flash.filesystem.File;

	import flashexporter.data.AppData;
	import flashexporter.data.Swf;
	import flashexporter.data.Symbol;

	import shared.air.utils.FileUtil;

	public class ExportBundleCommand extends ProcessingCommandBase
	{
		private var _queue:CommandQueue;
		private var _swf:Swf;

		public function ExportBundleCommand(swf:Swf)
		{
			_swf = swf;
		}

		override protected function onExecute():void
		{
			_queue = createQueue();
			_queue.execute();
		}

		private function createQueue():CommandQueue
		{
			var isAnyTextureSelected:Boolean = appData.generateTextures
					&& from(_swf.symbols).where(Symbol.isTextureProp).exists();

			var queue:CommandQueue = new CommandQueue();
			queue.onComplete(dispatchComplete);

			/** do not clear dir due to incremental generating*/
//			queue.add(new CleanDirCommand(_bundle.outputDir));

			for each (var symbol:Symbol in _swf.symbols)
			{
				if (symbol.isProcessed)
					continue;

				if (symbol.isTexture && !isAnyTextureSelected)
					continue;


				queue.add(new ProcessSymbolCommand(symbol));
			}

			if (isAnyTextureSelected)
				queue.add(new CreateSheetsCommand(_swf));

			queue.add(new AsyncWrapper(writeTimeline));

			queue.add(new GenerateSourceCommand(_swf));

			queue.add(new AsyncWrapper(touchBundleFile));

			return  queue;
		}

		private function touchBundleFile():void
		{
			var file:File = app.outputDir
					.resolvePath(_swf.bundleName)
					.resolvePath(".bundle");

			FileUtil.writeText(file, "");
		}

		private function writeTimeline():void
		{
			if (appData.isProcessingFailed)
				return;

			var timelineSymbols:Array = from(_swf.symbols)
					.where(Symbol.isClassProp)
					.select();

			if (timelineSymbols.length > 0)
			{
				app.logger.debug("writing timeline...");
				var description:XML = AppData.getDescription(timelineSymbols);
				var file:File = app.outputDir
						.resolvePath(_swf.bundleName)
						.resolvePath("timeline");
				app.writeBundleDescription(file, description);
			}
		}

		override protected function onCancel():void
		{
			_queue.cancel();
		}
	}
}
