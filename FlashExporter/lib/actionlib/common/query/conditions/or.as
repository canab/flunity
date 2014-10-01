package actionlib.common.query.conditions
{
	public function or(...conditions):Function
	{
		return function(item:*):Boolean
		{
			for each (var condition:Function in conditions)
			{
				if (condition(item))
					return true;
			}

			return false;
		};

	}
}
