package actionlib.common.query.iterators
{
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.utils.Proxy;
	import flash.utils.flash_proxy;

	public class DisplayTreeIterator extends Proxy
	{
		//noinspection JSFieldCanBeLocal
		private var _stack:Array;
		private var _target:DisplayObjectContainer;

		private var _childNum:int = 0;
		private var _childCount:int = 0;

		public function DisplayTreeIterator(target:DisplayObjectContainer)
		{
			_stack = [target];
		}

		override flash_proxy function nextNameIndex(index:int):int
		{
			if (_childNum < _childCount)
				return 1;
			
			if (_stack.length == 0)
				return 0;

			_target = _stack.pop();
			_childCount = _target.numChildren;
			_childNum = 0;

			return _childNum < _childCount ? 1 : 0;
		}

		override flash_proxy function nextValue(index:int):*
		{
			var nextValue:DisplayObject = _target.getChildAt(_childNum++);

			if (nextValue is DisplayObjectContainer && DisplayObjectContainer(nextValue).numChildren > 0)
				_stack.push(nextValue);

			return nextValue;
		}
	}
}
