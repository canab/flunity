package flashexporter.commands
{
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.commands.ICancelableCommand;
	import actionlib.common.logging.LogLevel;
	import actionlib.common.logging.adapters.TextFieldLogAdapter;

	import flash.desktop.NativeProcess;
	import flash.desktop.NativeProcessStartupInfo;
	import flash.events.NativeProcessExitEvent;
	import flash.events.ProgressEvent;
	import flash.filesystem.File;

	import flashexporter.abstracts.AsyncAppCommand;

	import shared.air.utils.FileUtil;

	public class RunExternalCommand extends AsyncAppCommand implements ICancelableCommand
	{
		private var _command:File;
		private var _process:NativeProcess;

		public function RunExternalCommand(command:File)
		{
			_command = command;
		}

		override public function execute():void
		{
			app.logger.info("Running", _command.nativePath + ":");

			if (!_command.exists)
			{
				app.logger.warn("File not found");
				new CallLaterCommand(dispatchComplete).execute();
				return;
			}

			var processInfo:NativeProcessStartupInfo = new NativeProcessStartupInfo();
			processInfo.executable = _command;
			processInfo.workingDirectory = File.applicationDirectory;

			_process = new NativeProcess();
			_process.addEventListener(NativeProcessExitEvent.EXIT, onProcessExit);
			_process.addEventListener(ProgressEvent.STANDARD_ERROR_DATA, onErrorData);
			_process.addEventListener(ProgressEvent.STANDARD_OUTPUT_DATA, onOutputData);
			_process.start(processInfo);
		}

		public function onOutputData(event:ProgressEvent):void
		{
			var data:String = _process.standardOutput.readUTFBytes(_process.standardOutput.bytesAvailable);
			TextFieldLogAdapter(app.logger.adapter).append(LogLevel.INFO, data);
		}

		public function onErrorData(event:ProgressEvent):void
		{
			var data:String = _process.standardError.readUTFBytes(_process.standardError.bytesAvailable);
			TextFieldLogAdapter(app.logger.adapter).append(LogLevel.ERROR, data);
		}

		private function onProcessExit(event:NativeProcessExitEvent):void
		{
			_process = null;
			dispatchComplete();
		}

		public function cancel():void
		{
			if (_process)
			{
				_process.exit(true);
				_process = null;
			}
		}
	}
}
