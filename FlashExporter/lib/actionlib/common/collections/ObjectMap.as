package actionlib.common.collections
{
	import actionlib.common.utils.MapUtil;

	import flash.utils.Dictionary;

	public dynamic class ObjectMap extends Dictionary
	{
		public static function copyOf(source:Object):ObjectMap
		{
			var result:ObjectMap = new ObjectMap();
			MapUtil.copyProperties(result, source);
			return result;
		}

		//noinspection JSUnusedLocalSymbols
		public function ObjectMap(keyType:Class = null, valueType:Class = null, weakKeys:Boolean = false)
		{
			super(weakKeys);
		}

		public function removeKey(key:Object):void
		{
			delete this[key];
		}

		public function removeValue(value:*):void
		{
			MapUtil.removeValue(this, value);
		}

		public function getLength():int
		{
			return MapUtil.getLength(this);
		}

		public function getKeys():Array
		{
			return MapUtil.getKeys(this);
		}

		public function getValues():Array
		{
			return MapUtil.getValues(this);
		}

		public function isEmpty():Boolean
		{
			return MapUtil.isEmpty(this);
		}

		public function containsKey(key:Object):Boolean
		{
			return MapUtil.containsKey(this, key);
		}

		public function containsValue(value:*):Boolean
		{
			return MapUtil.containsValue(this, value);
		}
	}
}
