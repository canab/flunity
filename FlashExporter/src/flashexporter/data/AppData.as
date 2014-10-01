package flashexporter.data
{
	import actionlib.common.events.EventSender;
	import actionlib.common.query.from;

	import flash.events.TimerEvent;
	import flash.filesystem.File;
	import flash.utils.Timer;

	import flashexporter.abstracts.AppContext;

	public class AppData extends AppContext
	{
		public static function getDescription(symbols:Object):XML
		{
			var xml:XML = <resources/>;
			for each (var symbol:Symbol in symbols)
			{
				xml.appendChild(symbol.getDescription());
			}
			return xml;
		}

		public var fileChangedEvent:EventSender = new EventSender(this);

		[Bindable]
		public var autoGenerateEnabled:Boolean = false;

		[Bindable]
		public var projectDir:File;

		[Bindable]
		public var generateTextures:Boolean = false;

		[Bindable]
		public var showStats:Boolean = false;

		public var files:Vector.<Swf> = new <Swf>[];

		public var symbols:Vector.<Symbol> = new <Symbol>[];

		public var isProcessingFailed:Boolean;

		public function AppData()
		{
			reset();

			var timer:Timer = new Timer(1000);
			timer.addEventListener(TimerEvent.TIMER, onTimer);
			timer.start();
		}

		public function getSwfByPath(nativePath:String):Swf
		{
			for each (var swf:Swf in files)
			{
				if (swf.file.nativePath == nativePath)
					return swf;
			}
			return null;
		}

		public function getChangedFiles():Vector.<Swf>
		{
			var result:Vector.<Swf> = new <Swf>[];
			for each (var swf:Swf in files)
			{
				if (swf.isChanged)
					result.push(swf);
			}
			return result;
		}

		public function getAllSymbols(fromFiles:Vector.<Swf>):Vector.<Symbol>
		{
			var result:Vector.<Symbol> = new <Symbol>[];
			for each (var file:Swf in fromFiles)
			{
				result = result.concat(file.symbols);
			}
			return  result;
		}

		public function reset():void
		{
			files = new <Swf>[];
			symbols = new <Symbol>[];
		}

		private function onTimer(event:TimerEvent):void
		{
			if (!autoGenerateEnabled)
				return;

			if (getChangedFiles().length > 0)
				fileChangedEvent.dispatch();
		}

		public function getFileByBundleName(bundleName:String):Swf
		{
			return from(appData.files)
					.where(function(it:Swf):Boolean {
						return it.bundleName == bundleName })
					.findFirst();
		}
	}
}
