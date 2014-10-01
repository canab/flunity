package actionlib.common.query
{
	import actionlib.common.errors.NullPointerError;

	public function from(collection:Object):Query
	{
		if (!collection)
			throw new NullPointerError();

		return new Query(collection);
	}
}
