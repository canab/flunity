package actionlib.common.utils
{
	public class MapUtil
	{
		public static function copyProperties(target:Object, source:Object):Object
		{
			for (var key:String in source)
			{
				target[key] = source[key];
			}
			return source;
		}

		public static function removeValue(map:Object, value:*):void
		{
			for (var key:Object in map)
			{
				if (map[key] === value)
				{
					delete map[key];
					return;
				}
			}
		}

		public static function getLength(object:Object):int
		{
			var length:int = 0;
			//noinspection JSUnusedLocalSymbols
			for each (var item:Object in object)
			{
				length++;
			}
			return length;
		}

		public static function getKeys(object:Object):Array
		{
			var result:Array = [];
			for (var key:Object in object)
			{
				result.push(key);
			}
			return result;
		}

		public static function getValues(object:Object):Array
		{
			var result:Array = [];
			for each (var item:Object in object)
			{
				result.push(item);
			}
			return result;
		}

		public static function isEmpty(object:Object):Boolean
		{
			//noinspection LoopStatementThatDoesntLoopJS,JSUnusedLocalSymbols
			for each (var item:Object in object)
			{
				return false;
			}
			return true;
		}

		public static function containsKey(object:Object, key:Object):Boolean
		{
			return (key in object);
		}

		public static function containsValue(object:Object, value:*):Boolean
		{
			for each (var item:Object in object)
			{
				if (item === value)
					return true;
			}
			
			return false;
		}
	}
}
