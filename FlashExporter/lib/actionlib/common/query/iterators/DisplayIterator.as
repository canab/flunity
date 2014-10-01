package actionlib.common.query.iterators
{
	import flash.display.DisplayObjectContainer;
	import flash.utils.flash_proxy;
	import flash.utils.Proxy;

	public class DisplayIterator extends Proxy
	{
		private var _target:DisplayObjectContainer;

		private var _childNum:int = 0;
		private var _childCount:int = 0;

		public function DisplayIterator(target:DisplayObjectContainer)
		{
			_target = target;
		}

		override flash_proxy function nextNameIndex(index:int):int
		{
			if (_childCount == 0)
				_childCount = _target.numChildren;

			return _childNum < _childCount ? 1 : 0;
		}

		override flash_proxy function nextValue(index:int):*
		{
			return _target.getChildAt(_childNum++);
		}
	}
}
