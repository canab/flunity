package flashexporter.data
{
	import actionlib.common.resources.loaders.SWFLoader;
	import actionlib.common.utils.CompareUtils;
	import actionlib.common.utils.DisplayUtil;
	import actionlib.common.utils.ReflectUtil;
	import actionlib.common.utils.StringUtil;

	import flash.display.DisplayObjectContainer;
	import flash.filesystem.File;
	import flash.system.ApplicationDomain;
	import flash.system.LoaderContext;

	import ru.etcs.utils.getDefinitionNames;

	import shared.air.utils.FileUtil;

	public class Swf
	{
		private var _name:String;
		private var _file:File;
		private var _loader:SWFLoader;
		private var _symbols:Vector.<Symbol>;
		private var _creationTime:Number;
		private var _modificationTime:Number;
		private var _bundleName:String;
		private var _sheetParams:SheetParams;

		public function Swf(file:File)
		{
			_file = file;
			_name = FileUtil.getNameWithoutExt(_file);
			_bundleName = _name;
			_sheetParams = new SheetParams(FileUtil.swapExt(file, "xml"));
		}

		public function createLoader():SWFLoader
		{
			unload();

			_loader = new SWFLoader(_file.url);
			_loader.loaderContext = new LoaderContext(false, new ApplicationDomain());
			_loader.onComplete(onLoadComplete);

			return _loader;
		}

		public function unload():void
		{
			if (_loader)
			{
				_loader.nativeLoader.unloadAndStop();
				_loader = null;
			}
		}

		private function onLoadComplete():void
		{
			if (isChanged)
			{
				readSymbols();
				updateTimeStamp();
			}
		}

		private function updateTimeStamp():void
		{
			_creationTime = _file.creationDate.time;
			_modificationTime = _file.modificationDate.time;
		}

		private function readSymbols():void
		{
			var newSymbols:Vector.<Symbol> = new <Symbol>[];

			var definitions:Array = getDefinitionNames(_loader.loaderInfo, false, true);

			for each (var qualifiedName:String in definitions)
			{
				if (StringUtil.endsWith(qualifiedName, "MainTimeline"))
					continue;

				qualifiedName = qualifiedName.replace("::", ".");

				if (StringUtil.startsWith(qualifiedName, _bundleName + "."))
				{
					newSymbols.push(new Symbol(qualifiedName));
					continue;
				}

				if (qualifiedName.indexOf(".") < 0)
				{
					newSymbols.push(new Symbol(qualifiedName, _bundleName));
					continue;
				}
			}

			newSymbols.sort(function(a:Symbol, b:Symbol):int
			{
				return CompareUtils.compareStrings(a.id, b.id);
			});

			_symbols = newSymbols;
		}

		public function createInstance(qualifiedName:String):Object
		{
			var instance:Object = ReflectUtil.createInstanceFromDomain(_loader.domain, qualifiedName);
			if (instance is DisplayObjectContainer)
				DisplayUtil.stopAllClips(DisplayObjectContainer(instance));
			return instance;
		}

		public function get pixelId():String
		{
			return bundleName + ".__pixel__";
		}

		public function toString():String
		{
			return "Swf{" + _name + "}";
		}

		public function get isLoaded():Boolean { return _loader != null; }

		public function get name():String { return _name; }

		public function get file():File { return _file; }

		public function get symbols():Vector.<Symbol> { return _symbols; }

		public function get bundleName():String { return _bundleName; }

		public function get sheetParams():SheetParams { return _sheetParams; }

		public function get isChanged():Boolean
		{
			return _creationTime != file.creationDate.time
				|| _modificationTime != file.modificationDate.time;
		}
	}
}
