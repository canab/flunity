package actionlib.common.collections
{
	import actionlib.common.utils.MapUtil;

	public dynamic class WeakObjectMap extends ObjectMap
	{
		public static function copyOf(source:Object):WeakObjectMap
		{
			var result:WeakObjectMap = new WeakObjectMap();
			MapUtil.copyProperties(result, source);
			return result;
		}

		public function WeakObjectMap(keyType:Class = null, valueType:Class = null)
		{
			super(keyType, valueType, true);
		}
	}
}
