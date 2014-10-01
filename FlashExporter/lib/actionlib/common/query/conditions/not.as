package actionlib.common.query.conditions
{
	public function not(condition:Function):Function
	{
		return function(item:*):Boolean
		{
			return !condition(item);
		};
	}
}
