package actionlib.common.query.conditions
{
	public function idIs(id:*):Function
	{
		return function(item:Object):Boolean
		{
			return item
				&& item.hasOwnProperty("id")
				&& item.id === id;
		};
	}
}
