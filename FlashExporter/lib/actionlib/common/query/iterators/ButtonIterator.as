package actionlib.common.query.iterators
{
	import actionlib.common.query.Query;
	import actionlib.common.query.fromDisplayTree;

	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.SimpleButton;
	import flash.utils.Proxy;
	import flash.utils.flash_proxy;

	public class ButtonIterator extends Proxy
	{
		public static function from(button:SimpleButton):Query
		{
			return actionlib.common.query.from(new ButtonIterator(button));
		}

		private var _items:Array = [];
		private var _index:int = 0;

		public function ButtonIterator(target:SimpleButton)
		{
			addItems(target.upState);
			addItems(target.overState);
			addItems(target.downState);
		}

		private function addItems(state:DisplayObject):void
		{
			_items.push(state);

			if (state is DisplayObjectContainer)
				fromDisplayTree(DisplayObjectContainer(state)).apply(_items.push);
		}

		override flash_proxy function nextNameIndex(index:int):int
		{
			return _index < _items.length ? 1 : 0;
		}

		override flash_proxy function nextValue(index:int):*
		{
			return _items[_index++];
		}
	}
}
