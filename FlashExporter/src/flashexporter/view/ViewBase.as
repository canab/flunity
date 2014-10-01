package flashexporter.view
{
	import actionlib.common.commands.IAsincCommand;
	import actionlib.common.commands.ICancelableCommand;
	import actionlib.common.query.from;

	import mx.collections.ArrayCollection;
	import mx.events.FlexEvent;

	import flashexporter.ToolApplication;
	import flashexporter.data.AppData;

	import spark.components.Group;

	public class ViewBase extends Group
	{
		[Bindable]
		public var actionEnabled:Boolean = false;

		[Bindable]
		protected var cancelEnabled:Boolean = false;

		[Bindable]
		protected var currentCommand:IAsincCommand;

		public function ViewBase()
		{
			addEventListener(FlexEvent.CREATION_COMPLETE, onCreationComplete);
		}

		private function onCreationComplete(event:FlexEvent):void
		{
			onInitialize();
			refreshState();
		}

		protected function toCollection(source:Object):ArrayCollection
		{
			return new ArrayCollection(from(source).select());
		}

		protected function runCommand(command:IAsincCommand):void
		{
			currentCommand = command;
			currentCommand.completeEvent.addListener(onCommandComplete);
			currentCommand.execute();
			refreshState();
		}

		private function onCommandComplete():void
		{
			currentCommand = null;
			refreshState();
		}

		protected function cancelCurrentCommand():void
		{
			var cancelableCommand:ICancelableCommand = currentCommand as ICancelableCommand;
			if (cancelableCommand)
				cancelableCommand.cancel();

			currentCommand = null;

			refreshState();
		}

		protected function cancelByUser():void
		{
			app.logger.warn("Canceled");
			cancelCurrentCommand();
		}

		protected function refreshState():void
		{
			/** need callLater due to strange bind-related crash in flex sgk in debug mode*/
			callLater(function():void
			{
				actionEnabled = !currentCommand;
				cancelEnabled = currentCommand is ICancelableCommand;
				app.view.refreshLog();
				refresh();
			})
		}

		public function reload():void
		{
		}

		protected function clearLog():void
		{
			app.view.clearLog();
		}

		public virtual function onKeyDown():void
		{
		}

		protected virtual function onInitialize():void
		{
		}

		protected virtual function refresh():void
		{
		}

		protected function get app():ToolApplication
		{
			return ToolApplication.instance;
		}

		protected function get appData():AppData
		{
			return ToolApplication.instance.appData;
		}
	}
}
