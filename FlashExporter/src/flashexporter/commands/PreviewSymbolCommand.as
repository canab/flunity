package flashexporter.commands
{
	import actionlib.common.commands.IAsincCommand;
	import actionlib.common.commands.ICancelableCommand;
	import flashexporter.rendering.ClipPrerenderer;

	import flash.display.Bitmap;

	import mx.core.UIComponent;

	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Symbol;
	import flashexporter.processing.ProcessSymbolCommand;
	import flashexporter.spritesheet.PreviewSheet;

	import spark.components.Window;

	public class PreviewSymbolCommand extends AsyncAppCommand implements ICancelableCommand
	{
		private var _symbol:Symbol;
		private var _command:IAsincCommand;

		public function PreviewSymbolCommand(symbol:Symbol)
		{
			_symbol = symbol;
		}

		override public function execute():void
		{
			ClipPrerenderer.resetStatistics();
			app.view.clearLog();
			appData.isProcessingFailed = false;

			_command = new ProcessSymbolCommand(_symbol);
			_command.completeEvent.addListener(onProcessComplete);
			_command.execute();
		}

		private function onProcessComplete():void
		{
			if (appData.isProcessingFailed)
				app.logger.error("Processing failed");
			else
				showPreview();

			dispatchComplete();
		}

		private function showPreview():void
		{
			if (_symbol.frames == null)
				return;

			var sheet:PreviewSheet = new PreviewSheet(_symbol.frames);
			if (!sheet.fill())
				return;

			var bitmap:Bitmap = new Bitmap(sheet.texture);
			var control:UIComponent = new UIComponent();
			control.addChild(bitmap);
			control.width = bitmap.width;
			control.height = bitmap.height;
			control.opaqueBackground = 0x808080;

			var window:Window = new Window();
			window.title = _symbol.id;
			window.showStatusBar = false;
			window.width = control.width;
			window.height = control.height;
			window.addElement(control);
			window.open();
		}

		public function cancel():void
		{
			if (_command is ICancelableCommand)
				(_command as ICancelableCommand).cancel();
		}
	}

}