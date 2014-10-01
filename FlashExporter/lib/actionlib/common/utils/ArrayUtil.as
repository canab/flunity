package actionlib.common.utils
{
	import actionlib.common.errors.ItemAlreadyExistsError;
	import actionlib.common.errors.ItemNotFoundError;

	public class ArrayUtil
	{
		public static function lastItem(source:Object):Object
		{
			if (source.length > 0)
				return source[source.length - 1];
			else
				throw new ItemNotFoundError();
		}

		public static function removeItem(source:Object, item:Object):Boolean
		{
			var index:int = source.indexOf(item);
			if (index >= 0)
			{
				source.splice(index, 1);
				return true;
			}
			return false;
		}
		
		public static function removeItems(source:Object, items:Array):int
		{
			var i:int = 0;
			var length:int = source.length;
			while (i < source.length)
			{
				var item:Object = source[i];
				var index:int = items.indexOf(item);
				if (index >= 0)
					source.splice(i, 1);
				else
					i++;
			}
			return length - source.length;
		}

		/**
		 * Removes the item from the array or vector.
		 * If array/vector does not contains this item, exception is thrown.
		 * @param source
		 * Vector or Array
		 * @param item
		 * Object
		 */
		public static function removeItemSafe(source:Object, item:Object):void
		{
			var index:int = source.indexOf(item);
			if (index >= 0)
				source.splice(index, 1);
			else
				throw new ItemNotFoundError();
		}

		/**
		 * Adds the item to the array or vector.
		 * If array/vector does contains this item, exception is thrown.
		 * @param source
		 * Vector or Array
		 * @param item
		 * Object
		 */
		public static function addItemSafe(source:Object, item:Object):void
		{
			var index:int = source.indexOf(item);
			if (index >= 0)
				throw new ItemAlreadyExistsError();
			else
				source.push(item);
		}
		
		public static function getRandomItem(source:Object):*
		{
			return source[int(Math.random() * source.length)];
		}

		public static function shuffle(source:Object):void
		{
			for (var i:int = 0; i < source.length; i++)
			{
				var newIndex:int = MathUtil.randomInt(0, source.length - 1);
				var value:* = source[i];
				source.splice(i, 1);
				source.splice(newIndex, 0, value);
			}
		}
		
		public static function getRandomItems(source:Object, count:int):Array
		{
			if (source.length < count)
				throw new Error("Source length is less then items requested");
			
			var result:Array = [];
			var selection:Array = [];
			
			for (var i:int = 0; i < count; i++)
			{
				var index:int = Math.random() * source.length;
				
				while(selection.indexOf(index) >= 0)
				{
					index++;
					if (index == source.length)
						index = 0;
				}
				
				result.push(source[index]);
				selection.push(index);
			}
			
			return result;
		}		
		
		public static function equals(source:Object, target:Object):Boolean
		{
			if (source == null && target == null)
				return true;
			
			if (source == null || target == null)
				return false;
			
			if (source.length != target.length)
				return false;
				
			for each (var item:Object in source) 
			{
				if (target.indexOf(item) == -1)
					return false;
			}
			
			return true;
		}
		
		public static function pushUniqueItem(target:Object, item:Object):Boolean
		{
			if (target.indexOf(item) == -1)
			{
				target.push(item);
				return true;
			}
			else
			{
				return false;
			}
		}
		
		public static function pushUniqueItems(target:Object, items:Array):int
		{
			var result:int = 0;
			for each (var item:Object in items) 
			{
				if (pushUniqueItem(target, item))
					result++;
			}
			return result;
		}

		public static function toObject(collection:Object, keyField:String):Object
		{
			var result:Object = {};
			for each (var item:Object in collection)
			{
				result[item[keyField]] = item;
			}
			return result;
		}

	}

}