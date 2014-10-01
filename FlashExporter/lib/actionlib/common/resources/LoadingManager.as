package actionlib.common.resources
{
	import actionlib.common.errors.ItemNotFoundError;
	import actionlib.common.resources.loaders.LoaderBase;
	import actionlib.common.utils.ArrayUtil;

	internal class LoadingManager
	{
		public static const MAX_LOADING_COUNT:int = 10;
		
		private var _loadersQueue:Vector.<LoaderBase> = new <LoaderBase>[];
		private var _executedLoaders:Vector.<LoaderBase> = new <LoaderBase>[];

		public function addLoader(loader:LoaderBase):void
		{
			_loadersQueue.push(loader);
			checkNextLoader();
		}

		public function removeLoader(loader:LoaderBase):void
		{
			if (_loadersQueue.indexOf(loader) >= 0)
			{
				ArrayUtil.removeItem(_loadersQueue, loader);
			}
			else if (_executedLoaders.indexOf(loader) >= 0)
			{
				loader.cancel();
				ArrayUtil.removeItem(_executedLoaders, loader);
				checkNextLoader();
			}
			else
			{
				throw new ItemNotFoundError();
			}
		}

		private function checkNextLoader():void
		{
			if (_executedLoaders.length < MAX_LOADING_COUNT && _loadersQueue.length > 0)
				loadNext();
		}

		private function loadNext():void
		{
			var loader:LoaderBase = _loadersQueue.shift();
			loader.completeEvent.addListener(onLoadComplete);
			loader.execute();

			_executedLoaders.push(loader);
		}

		private function onLoadComplete(loader:LoaderBase):void
		{
			ArrayUtil.removeItem(_executedLoaders, loader);
			loader.completeEvent.removeListener(onLoadComplete);
			checkNextLoader();
		}
	}

}
