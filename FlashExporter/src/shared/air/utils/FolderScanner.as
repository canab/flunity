package shared.air.utils
{
	import actionlib.common.utils.CompareUtils;

	import flash.filesystem.File;

	public class FolderScanner
	{
		private var _condition:Function = null;
		private var _rootDir:File;

		public function FolderScanner(rootDir:File)
		{
			_rootDir = rootDir;
		}

		public function where(condition:Function):FolderScanner
		{
			_condition = condition;
			return this;
		}

		public function findFiles():Vector.<File>
		{
			var result:Vector.<File> = (_rootDir.exists && _rootDir.isDirectory)
					? scanDir(_rootDir)
					: new <File>[];

			result.sort(fileSorter);

			return result;
		}

		public function findFilesRecursive():Vector.<File>
		{
			var result:Vector.<File> = (_rootDir.exists && _rootDir.isDirectory)
					? scanDirRecursive(_rootDir)
					: new <File>[];

			result.sort(fileSorter);

			return result;
		}

		private function scanDir(dir:File):Vector.<File>
		{
			var result:Vector.<File> = new <File>[];
			var files:Array = dir.getDirectoryListing();

			for each (var file:File in files)
			{
				if (_condition == null || _condition(file))
					result.push(file);
			}

			return result;
		}

		private function scanDirRecursive(dir:File):Vector.<File>
		{
			var result:Vector.<File> = new <File>[];
			var files:Array = dir.getDirectoryListing();

			for each (var file:File in files)
			{
				if (file.isDirectory)
					result = result.concat(scanDirRecursive(file));
				else if (_condition == null || _condition(file))
					result.push(file);
			}

			return result;
		}

		private function fileSorter(a:File, b:File):int
		{
			return CompareUtils.compareStrings(a.nativePath, b.nativePath);
		}

	}

}