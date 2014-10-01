package flashexporter.commands
{
	import actionlib.common.commands.CommandQueue;
	import actionlib.common.commands.ICancelableCommand;
	import flashexporter.rendering.ClipPrerenderer;

	import flash.desktop.NativeApplication;
	import flash.display.BitmapData;

	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Swf;
	import flashexporter.processing.ExportBundleCommand;

	public class ExportSelectionCommand extends AsyncAppCommand implements ICancelableCommand
	{
		private var _command:ICancelableCommand;
		private var _startupTime:Date;
		private var _frameRate:int;
		private var _files:Vector.<Swf>;

		public function ExportSelectionCommand(files: Vector.<Swf>)
		{
			_files = files;
		}

		override public function execute():void
		{
			initialize();

			_command = new ReloadDataCommand();
			_command.completeEvent.addListener(onReloadComplete);
			_command.execute();
		}

		private function onReloadComplete():void
		{
			var queue:CommandQueue = new CommandQueue();

			for each (var swf:Swf in _files)
			{
				queue.add(new ExportBundleCommand(swf));
			}

			_command = queue;
			_command.completeEvent.addListener(doComplete);
			_command.execute();
		}

		private function initialize():void
		{
			_startupTime = new Date();
			_frameRate = app.root.stage.frameRate;
			_frameRate = app.root.stage.frameRate = 500;

			ClipPrerenderer.resetStatistics();
			app.view.clearLog();
			appData.isProcessingFailed = false;

			NativeApplication.nativeApplication.icon.bitmaps = [new BitmapData(16, 16, true, 0xFF00FF00)];
		}

		private function doComplete():void
		{
			NativeApplication.nativeApplication.icon.bitmaps = [];

			if (appData.isProcessingFailed)
			{
				app.logger.error("Processing failed");
			}
			else
			{
				if (appData.showStats)
					app.showStats();
				var time:Number = new Date().time - _startupTime.time;
				app.logger.info("\nCompleted (#t sec)".replace("#t", time / 1000.0));
			}

			app.root.stage.frameRate = _frameRate;
			app.unloadSwf();
			dispatchComplete();
		}

		public function cancel():void
		{
			_command.cancel();
		}
	}

}