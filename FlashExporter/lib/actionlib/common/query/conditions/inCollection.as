package actionlib.common.query.conditions
{
	public function inCollection(collection:Object):Function
	{
		return function(item:Object):Boolean
		{
			for each (var collectionItem:Object in collection)
			{
				if (item === collectionItem)
					return true;
			}

			return false;
		};
	}
}
