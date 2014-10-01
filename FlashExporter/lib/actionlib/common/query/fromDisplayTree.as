package actionlib.common.query
{
	import actionlib.common.errors.NullPointerError;
	import actionlib.common.query.iterators.DisplayTreeIterator;

	import flash.display.DisplayObjectContainer;

	public function fromDisplayTree(target:DisplayObjectContainer):Query
	{
		if (!target)
			throw new NullPointerError();

		return new Query(new DisplayTreeIterator(target));
	}
}
