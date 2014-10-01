package actionlib.common.query
{
	import actionlib.common.errors.NullPointerError;
	import actionlib.common.query.iterators.DisplayIterator;

	import flash.display.DisplayObjectContainer;

	public function fromDisplay(target:DisplayObjectContainer):Query
	{
		if (!target)
			throw new NullPointerError();

		return new Query(new DisplayIterator(target));
	}
}
