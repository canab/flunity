package actionlib.common.query.conditions
{
	public function namePrefixIs(namePrefix:String):Function
	{
		return function(item:Object):Boolean
		{
			return item
				&& item.hasOwnProperty("name")
				&& String(item.name).indexOf(namePrefix) == 0;
		};
	}
}
