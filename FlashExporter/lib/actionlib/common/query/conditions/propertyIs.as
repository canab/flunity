package actionlib.common.query.conditions
{
	public function propertyIs(propName:String, value:*):Function
	{
		return function(item:Object):Boolean
		{
			return item
				&& item.hasOwnProperty(propName)
				&& item[propName] == value;
		};
	}
}
