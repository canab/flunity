package actionlib.common.query.mappers
{
	public function property(propName:String):Function
	{
		return function (item:Object):*
		{
			if (!item)
				return undefined;

			if (!item.hasOwnProperty(propName))
				return undefined;

			return item[propName];

		}
	}
}
