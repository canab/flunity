package actionlib.common.query.mappers
{
	public function idField(item:Object):*
	{
		if (!item)
			return null;

		if (!item.hasOwnProperty("id"))
			return null;

		return item.id;
	}
}
