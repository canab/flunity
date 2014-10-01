package flashexporter.commands
{
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.resources.loaders.SWFLoader;

	import flash.events.ProgressEvent;

	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Swf;

	public class LoadSwfCommand extends AsyncAppCommand
	{
		private const RETRY_TIME:int = 500;

		private var _swf:Swf;
		private var _retryCommand:CallLaterCommand;
		private var _loader:SWFLoader;
		private var _attemptNum:int = 0;

		public function LoadSwfCommand(swf:Swf)
		{
			_swf = swf;
		}

		override public function execute():void
		{
			app.unloadSwf();
			app.logger.debug("loading: ", _swf.file.name);
			loadSwf();
		}

		private function loadSwf():void
		{
			if (++_attemptNum > 1)
				app.logger.debug("retrying...");

			_loader = _swf.createLoader();
			_loader.onComplete(onLoadComplete);
			_loader.execute();

			_loader.nativeLoader.contentLoaderInfo.addEventListener(ProgressEvent.PROGRESS, onProgress);
		}

		private function onProgress(event:ProgressEvent):void
		{
			if (event.bytesLoaded == event.bytesTotal)
			{
				_retryCommand = new CallLaterCommand(loadSwf, RETRY_TIME);
				_retryCommand.execute();
			}
		}

		private function onLoadComplete(loader:SWFLoader):void
		{
			disposeCurrentActions();

			if (loader.successful)
			{
				dispatchComplete();
			}
			else
			{
				_retryCommand = new CallLaterCommand(loadSwf, RETRY_TIME);
				_retryCommand.execute();
			}
		}

		private function disposeCurrentActions():void
		{
			if (_retryCommand != null)
			{
				_retryCommand.cancel();
				_retryCommand = null;
			}

			if (_loader != null)
			{
				_loader.cancel();
				_loader = null;
			}
		}
	}
}
