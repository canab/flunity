package actionlib.common.query.conditions
{
	public function equals(value:*):Function
	{
		return function(item:*):Boolean
		{
			return item === value;
		};
	}
}
