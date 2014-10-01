package shared.air.data
{
	import flash.filesystem.File;
	import flash.utils.ByteArray;

	import mx.utils.Base64Decoder;
	import mx.utils.Base64Encoder;

	import shared.air.utils.FileUtil;

	public class XMLSettings
	{
		private var _file:File;
		private var _data:XML;
		private var _autoFlush:Boolean = false;

		public function XMLSettings(file:File)
		{
			_file = file;
			_data = (file.exists)
					? FileUtil.readXML(file)
					: <settings/>;
		}

		public function getNode(name:String):XML
		{
			var nodes:XMLList = _data[name];
			return nodes.length() > 0 ? nodes[0] : <{name}/>;
		}

		public function readInt(name:String, defaultValue:int = 0):int
		{
			return int(getValue(name, defaultValue));
		}

		public function writeInt(name:String, value:int):void
		{
			_data[name] = value;
			flushIfNeeded();
		}

		public function readNumber(name:String, defaultValue:Number = 0):Number
		{
			return Number(getValue(name, defaultValue));
		}

		public function writeNumber(name:String, value:Number):void
		{
			_data[name] = value;
			flushIfNeeded();
		}

		public function readString(name:String, defaultValue:String = null):String
		{
			var value:String = getValue(name, defaultValue);
			return value;
		}

		public function writeString(name:String, value:String):void
		{
			_data[name] = value;
			flushIfNeeded();
		}

		public function readBoolean(name:String, defaultValue:Boolean = false):Boolean
		{
			return String(getValue(name, defaultValue)) == "true";
		}

		public function writeBoolean(name:String, value:Boolean):void
		{
			_data[name] = (value) ? "true" : "false";
			flushIfNeeded();
		}

		private function flushIfNeeded():void
		{
			if (_autoFlush)
				flush();
		}

		public function flush():void
		{
			FileUtil.writeXML(_file, _data);
		}

		private function getValue(name:String, defaultValue:*):*
		{
			return (name in _data) ? _data[name] : defaultValue;
		}

		public function get autoFlush():Boolean
		{
			return _autoFlush;
		}

		public function set autoFlush(value:Boolean):void
		{
			_autoFlush = value;
		}

		public function writeObject(name:String, value:Object):void
		{
			var bytes:ByteArray = new ByteArray();
			bytes.writeObject(value);

			var encoder:Base64Encoder = new Base64Encoder();
			encoder.encodeBytes(bytes);
			_data[name] = encoder.toString();

			flushIfNeeded();
		}

		public function readObject(name:String, defaultValue:Object):Object
		{
			var value:String = getValue(name, defaultValue);
			if (!value || value === defaultValue)
				return defaultValue;

			var decoder:Base64Decoder = new Base64Decoder();
			decoder.decode(value);

			return decoder.toByteArray().readObject();
		}
	}
}
