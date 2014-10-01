package flashexporter.processing
{
	import flash.filesystem.File;

	public class CleanDirCommand extends ProcessingCommandBase
	{
		private var _dir:File;

		public function CleanDirCommand(dir:File)
		{
			_dir = dir;
		}

		override protected function onExecute():void
		{
			app.logger.debug("cleanup:", _dir.nativePath);

			var error:Error = null;

			if (_dir.exists)
			{
				try
				{
					_dir.deleteDirectory(true);
					_dir.createDirectory();
				}
				catch (e:Error)
				{
					error = e;
				}
			}

			if (error)
				fail(error.message);
			else
				dispatchCompleteAsync();
		}
	}
}
