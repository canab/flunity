package actionlib.common.utils
{
	public class StringUtil
	{
		public static const EMPTY_CHARS:String = ' \t\r\n\f' + String.fromCharCode(160);

		public static function escape(text:String):String
		{
			return text
					.replace(/\\/g, "\\\\")
					.replace(/"/g, '\\"')
					.replace(/\r[\n]*/g, '\\n')
					.replace(/\t/g, '\\t"');
		}

		public static function endsWith(source:String, suffix:String):Boolean
		{
			if (source == null || source.length < suffix.length)
				return false;

			return source.substr(source.length - suffix.length) == suffix;
		}

		public static function startsWith(source:String, prefix:String):Boolean
		{
			if (source == null || source.length < prefix.length)
				return false;

			return source.substr(0, prefix.length) == prefix;
		}

		public static function upperFirstChar(source:String):String
		{
			return source.length == 0
					? source
					: source.substr(0, 1).toUpperCase() + source.substr(1);
		}

		public static function lowerFirstChar(source:String):String
		{
			return source.length == 0
					? source
					: source.substr(0, 1).toLowerCase() + source.substr(1);
		}

		public static function replaceChars(source:String, characters:Array, matches:Array):String
		{
			var result:String = '';

			for (var i:int = 0; i < source.length; i++)
			{
				var matchIndex:int = characters.indexOf(source.charAt(i));

				if (matchIndex != -1)
					result += matches[matchIndex];
				else
					result += source.charAt(i);
			}

			return result;
		}


		public static function isBlankString(source:String):Boolean
		{
			for (var i:int = 0; i <= source.length; i++)
			{
				if (EMPTY_CHARS.indexOf(source.charAt(i)) < 0)
					return false;
			}

			return true;
		}

		public static function trim(str:String):String
		{
			if (str == null)
				return '';

			var index1:int = 0;
			var index2:int = str.length - 1;
			var length:int = str.length;

			while (index1 < length && EMPTY_CHARS.indexOf(str.charAt(index1)) >= 0)
			{
				index1++;
			}

			while (index2 > 0 && EMPTY_CHARS.indexOf(str.charAt(index2)) >= 0)
			{
				index2--;
			}


			return (index2 >= index1)
				? str.slice(index1, index2 + 1)
				: '';
		}

		public static function format(str:String, ... args):String
		{
			var length:int = args.length;
			for (var i:int = 0; i < length; i++)
			{
				str = str.replace(new RegExp('\\{' + i + '\\}', 'g'), args[i]);
			}
			return str;
		}

		public static function extractInt(source:String):int
		{
			return source.match(/\d+/)[0];
		}
	}
	
}