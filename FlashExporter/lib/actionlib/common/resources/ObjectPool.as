package actionlib.common.resources
{
	public class ObjectPool
	{
		private var _pool:Array = [];
		private var _currentIndex:int = -1;

		private var _factoryMethod:Function;
		private var _resetMethod:Function;

		public function ObjectPool(factoryMethod:Function, resetMethod:Function = null)
		{
			_factoryMethod = factoryMethod;
			_resetMethod = resetMethod;
		}

		public function getObject():*
		{
			var object:*;
			
			if (_currentIndex >= 0)
			{
				object = _pool[_currentIndex];
				_pool[_currentIndex--] = null;
			}
			else
			{
				object = _factoryMethod();
			}

			return object;
		}

		public function putObject(object:*):void
		{
			if (_resetMethod != null)
				_resetMethod(object);

			_pool[++_currentIndex] = object;
		}
	}
}
