package actionlib.common.logging.adapters
{
	import actionlib.common.collections.ObjectMap;
	import actionlib.common.logging.ILogAdapter;
	import actionlib.common.logging.LogLevel;

	import flash.text.TextField;
	import flash.text.TextFormat;

	public class TextFieldLogAdapter implements ILogAdapter
	{
		private var _field:TextField;
		private var _formats:ObjectMap = new ObjectMap(LogLevel, TextFormat);

		public function TextFieldLogAdapter(field:TextField)
		{
			_field = field;

			_formats[LogLevel.INFO] = _field.getTextFormat();
			_formats[LogLevel.DEBUG] = _field.getTextFormat();
			_formats[LogLevel.WARN] = _field.getTextFormat();
			_formats[LogLevel.ERROR] = _field.getTextFormat();

			setColors();
		}

		public function setColors(
				debugColor:uint = 0xAAAAAA,
				infoColor:uint = 0x222222,
				warnColor:uint = 0xD56A00,
				errorColor:uint = 0xAA0000):void
		{
			TextFormat(_formats[LogLevel.DEBUG]).color = debugColor;
			TextFormat(_formats[LogLevel.INFO]).color = infoColor;
			TextFormat(_formats[LogLevel.WARN]).color = warnColor;
			TextFormat(_formats[LogLevel.ERROR]).color = errorColor;
		}

		public function print(sender:Object, level:LogLevel, message:String):void
		{
			message = message.replace(/\r\n/g, "\n");

			if (_field.text.length > 0)
				_field.appendText("\n");

			_field.appendText(message);
			_field.setTextFormat(_formats[level], _field.text.length - message.length - 1, _field.text.length);
			_field.scrollV = _field.maxScrollV;
		}

		public function append(level:LogLevel, message:String):void
		{
			message = message.replace(/\r\n/g, "\n");

			_field.appendText(message);
			_field.setTextFormat(_formats[level], _field.text.length - message.length - 1, _field.text.length);
			_field.scrollV = _field.maxScrollV;
		}
	}
}
