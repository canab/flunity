package actionlib.common.query.conditions
{
	public function and(...conditions):Function
	{
		return function(item:*):Boolean
		{
			for each (var condition:Function in conditions)
			{
				if (!condition(item))
					return false;
			}

			return true;
		};

	}
}
