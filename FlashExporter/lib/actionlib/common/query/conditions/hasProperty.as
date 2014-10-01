package actionlib.common.query.conditions
{
	public function hasProperty(propName:String):Function
	{
		return function(item:Object):Boolean
		{
			return item && item.hasOwnProperty(propName);
		}
	}
}
