package actionlib.common.query
{
	import actionlib.common.errors.NullPointerError;

	public class Query
	{
		//noinspection JSUnusedLocalSymbols
		public static function trueCondition(item:*):Boolean
		{
			return true;
		}

		public static function selfMapper(item:*):*
		{
			return item;
		}

		private var _collection:Object;
		private var _condition:Function = trueCondition;
		private var _mapper:Function = selfMapper;

		public function Query(collection:Object)
		{
			if (collection == null)
				throw new NullPointerError();

			_collection = collection;
		}

		public function where(condition:Function):Query
		{
			_condition = condition || trueCondition;
			return this;
		}

		public function map(mapper:Function):Query
		{
			_mapper = mapper || selfMapper;
			return this;
		}

		public function exists():Boolean
		{
			return findFirst() != undefined;
		}

		public function count():int
		{
			var result:int = 0;

			for each (var item:* in _collection)
			{
				if (_condition(item))
					result++;
			}

			return result;
		}

		public function findFirst(mapper:Function = null):*
		{
			map(mapper);

			for each (var item:* in _collection)
			{
				if (_condition(item))
					return _mapper(item);
			}

			return undefined;
		}

		public function findLast(mapper:Function = null):*
		{
			map(mapper);

			var result:* = undefined;
			for each (var item:* in _collection)
			{
				if (_condition(item))
					result = _mapper(item);
			}

			return result;
		}

		public function apply(func:Function):void
		{
			for each (var item:* in _collection)
			{
				if (_condition(item))
					func(_mapper(item));
			}
		}

		public function select(mapper:Function = null):Array
		{
			map(mapper);

			var result:Array = [];

			for each (var item:* in _collection)
			{
				if (_condition(item))
					result.push(_mapper(item));
			}

			return result;
		}

		public function selectRange(maxResult:int, skipCount:int = 0):Array
		{
			var result:Array = [];
			var skipNum:int = 0;

			for each (var item:* in _collection)
			{
				if (!_condition(item))
					continue;

				if (skipNum < skipCount)
				{
					skipNum++;
					continue;
				}

				result.push(_mapper(item));

				if (result.length == maxResult)
					break;
			}

			return result;
		}

		public function selectUnique(mapper:Function = null):Array
		{
			map(mapper);

			var result:Array = [];

			for each (var item:* in _collection)
			{
				if (!_condition(item))
					continue;

				var mappedItem:* = _mapper(item);

				if (result.indexOf(mappedItem) == -1)
					result.push(mappedItem);
			}

			return result;
		}

		public function aggregate(aggregator:Function):*
		{
			if (aggregator == null)
				throw new NullPointerError();

			var result:* = undefined;
			var isFirstItem:Boolean = true;

			for each (var item:* in _collection)
			{
				if (!_condition(item))
					continue;

				if (isFirstItem)
				{
					result = _mapper(item);
					isFirstItem = false;
				}
				else
				{
					result = aggregator(result, _mapper(item));
				}
			}

			return result;
		}
	}
}
