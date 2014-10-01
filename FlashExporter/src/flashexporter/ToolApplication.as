package flashexporter
{
	import actionlib.common.display.StageReference;
	import actionlib.common.logging.LogLevel;
	import actionlib.common.logging.Logger;
	import actionlib.common.resources.loaders.LoaderBase;
	import actionlib.common.utils.StringUtil;
	import flashexporter.rendering.ClipPrerenderer;

	import by.blooddy.crypto.image.PNGEncoder;

	import flash.display.BitmapData;
	import flash.filesystem.File;

	import flashexporter.data.AppData;
	import flashexporter.data.Swf;
	import flashexporter.data.UserSettings;
	import flashexporter.spritesheet.SheetUtil;

	import flashexporter.view.MainView;

	import shared.air.utils.FileUtil;

	public class ToolApplication
	{
		static private var _instance:ToolApplication;

		static public function createInstance(root:Main):void
		{
			if (_instance)
				throw new Error("Instance is already created.");

			StageReference.initialize(root.stage);

			_instance = new ToolApplication();
			_instance.initialize(root);
		}

		public static function get instance():ToolApplication
		{
			return _instance;
		}

		//-- instance --//

		Logger.defaultFormatter = new LogFormatter();
		Logger.setLevel(LoaderBase, LogLevel.NONE);


		[Bindable]
		public var appData:AppData = new AppData();

		[Bindable]
		public var processingScale:Number = 1;

		private var _root:Main;
		private var _view:MainView;
		private var _logger:Logger = new Logger(ToolApplication);
		private var _userSettings:UserSettings = new UserSettings();

		private function initialize(root:Main):void
		{
			readUserSettings();

			ClipPrerenderer.defaultFillColor = SheetUtil.FILL_COLOR;
			ClipPrerenderer.boundsClipName = AppConstants.BOUNDS_NAME;
			ClipPrerenderer.boundsExpandSize = 16;

			_root = root;
			_root.addElement(_view = new MainView());
		}

		public function writeBundleTexture(file:File, bitmap:BitmapData):void
		{
			file = FileUtil.swapExt(file, "png.bytes");
			FileUtil.writeBinary(file, PNGEncoder.encode(bitmap));
		}

		public function writeBundleDescription(file:File, content:XML):void
		{
			file = FileUtil.swapExt(file, "xml");
			FileUtil.writeXML(file, content);
		}

		public function showStats():void
		{
			logger.info("\nSprite stats:\n");
			logger.debug(ClipPrerenderer.getDetailedStats());
		}

		public static function correctFontName(fontName:String):String
		{
			//mac os fix
			var boldSuffix:String = " Bold";
			if (StringUtil.endsWith(fontName, boldSuffix))
				fontName = fontName.substr(0, fontName.length - boldSuffix.length);

			return fontName;
		}

		public function unloadSwf():void
		{
			for each (var file:Swf in appData.files)
			{
				file.unload();
			}
		}

		public function get flashDir():File
		{
			return new File(_userSettings.projectPath).resolvePath('Flash');
		}

		public function get outputDir():File
		{
			return new File(_userSettings.projectPath).resolvePath('Assets/Resources/FlashBundles');
		}

		public function readUserSettings():void
		{
			appData.projectDir = _userSettings.projectPath
				? new File(_userSettings.projectPath)
				: null;
			appData.generateTextures = _userSettings.generateTextures;
			appData.autoGenerateEnabled = _userSettings.autoGenerateEnabled;
		}

		public function saveUserSettings():void
		{
			_userSettings.projectPath = appData.projectDir
				? appData.projectDir.nativePath
				: null;
			_userSettings.generateTextures = appData.generateTextures;
			_userSettings.autoGenerateEnabled = appData.autoGenerateEnabled;
			_userSettings.flush();
		}


		public function get root():Main { return _root; }

		public function get logger():Logger { return _logger; }

		public function get view():MainView
		{
			return _view;
		}

	}

}

