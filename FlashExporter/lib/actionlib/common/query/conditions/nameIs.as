package actionlib.common.query.conditions
{
	public function nameIs(name:String):Function
	{
		return function(item:Object):Boolean
		{
			return item
				&& item.hasOwnProperty("name")
				&& item.name == name;
		};
	}
}
