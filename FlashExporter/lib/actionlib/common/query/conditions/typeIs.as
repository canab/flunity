package actionlib.common.query.conditions
{
	public function typeIs(type:Class):Function
	{
		return function(item:*):Boolean
		{
			return item is type;
		};
	}
}
