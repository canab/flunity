package actionlib.common.query.mappers
{
	public function newObject(type:Class):Function
	{
		return function(item:*):Object
		{
			return new type(item);
		}
	}
}
