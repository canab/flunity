package shared.air.utils
{
	import flash.filesystem.File;
	import flash.filesystem.FileMode;
	import flash.filesystem.FileStream;
	import flash.system.Capabilities;
	import flash.utils.ByteArray;

	public class FileUtil
	{
		public static function extensionIs(ext:String):Function
		{
			return function(file:File):Boolean
			{
				return file.extension.toLowerCase() == ext.toLowerCase();
			}
		}
		
		public static function replaceNativeEol(text:String):String
		{
			var os:String = Capabilities.os.toLowerCase();
			var eol:String = os.indexOf("windows") >= 0 ? "\r\n" : "\n";
			return text.replace(/\r\n/g, "\n")
					.replace(/\n/g, eol);
		}

		public static function getRelativeAppFile(relativePath:String):File
		{
			return new File(File.applicationDirectory.nativePath + "/" + relativePath);
		}

		public static function swapExt(source:File, ext:String):File
		{
			if (ext.length > 0 && ext.charAt(0) != ".")
				ext = "." + ext;

			var fileName:String = getNameWithoutExt(source);
			return new File(source.parent.nativePath).resolvePath(fileName + ext);
		}

		public static function getNameWithoutExt(file:File):String
		{
			var name:String = file.name;
			var dotIndex:int = name.lastIndexOf(".");
			if (dotIndex >= 0)
				name = name.substr(0, dotIndex);

			return name;
		}

		public static function readXML(file:File):XML
		{
			return XML(readText(file));
		}

		public static function readText(file:File):String
		{
			var stream:FileStream = new FileStream();
			stream.open(file, FileMode.READ);
			var result:String = stream.readUTFBytes(stream.bytesAvailable);
			stream.close();

			return result;
		}

		public static function readBytes(file:File):ByteArray
		{
			var result:ByteArray = new ByteArray();

			var stream:FileStream = new FileStream();
			stream.open(file, FileMode.READ);
			stream.readBytes(result);
			stream.close();

			return result;
		}

		public static function writeXML(file:File, data:XML):void
		{
			writeText(file, data.toXMLString());
		}

		public static function writeText(file:File, text:String):void
		{
			var tempFile:File = File.createTempFile();

			var stream:FileStream = new FileStream();
			stream.open(tempFile, FileMode.WRITE);
			stream.writeUTFBytes(text);
			stream.close();

			tempFile.moveTo(file, true);
		}

		public static function writeBinary(file:File, data:ByteArray):void
		{
			var tempFile:File = File.createTempFile();

			var stream:FileStream = new FileStream();
			stream.open(tempFile, FileMode.WRITE);
			stream.writeBytes(data);
			stream.close();

			tempFile.moveTo(file, true);
		}

		public static function createDirForFile(file:File):void
		{
			var dir:File = file.parent;
			if (!dir.exists)
				dir.createDirectory();
		}
	}
}
