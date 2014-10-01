package flashexporter.abstracts
{
	import actionlib.common.commands.AsincCommand;
	import actionlib.common.commands.CallLaterCommand;

	import flashexporter.ToolApplication;
	import flashexporter.data.AppData;

	public class AsyncAppCommand extends AsincCommand
	{
		//region static

		protected static function get app():ToolApplication
		{
			return ToolApplication.instance;
		}

		protected static function get appData():AppData
		{
			return ToolApplication.instance.appData;
		}

		//endregion

		protected function fail(message:String = null):void
		{
			if (message)
				app.logger.error(message);

			appData.isProcessingFailed = true;
			new CallLaterCommand(dispatchComplete).execute();
		}
	}
}