package actionlib.common.query.mappers
{
	import actionlib.common.utils.ReflectUtil;

	public function typedCopy(type:Class):Function
	{
		return function(item:*):Object
		{
			var result:Object = new type();
			ReflectUtil.copyFieldsAndProperties(item, result);
			return result;
		}
	}
}
