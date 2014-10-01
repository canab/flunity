package flashexporter.processing
{
	import actionlib.common.commands.ICancelableCommand;

	import flashexporter.abstracts.AsyncAppCommand;

	public class ProcessingCommandBase extends AsyncAppCommand  implements ICancelableCommand
	{
		override public final function execute():void
		{
			if (appData.isProcessingFailed)
				dispatchCompleteAsync();
			else
				onExecute();
		}

		public final function cancel():void
		{
			onCancel();
		}

		protected function onExecute():void
		{
		}

		protected function onCancel():void
		{
		}
	}
}
