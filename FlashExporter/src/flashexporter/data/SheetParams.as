package flashexporter.data
{
	import flash.filesystem.File;
	import shared.air.data.XMLSettings;

	public class SheetParams
	{
		private var _repo:XMLSettings;

		public function SheetParams(file:File)
		{
			_repo = new XMLSettings(file);
		}

		public function get size():int
		{
			return _repo.readInt("size", 1024);
		}

		public function set size(value:int):void
		{
			_repo.writeInt("size", value);
		}

		public function get size2x():int
		{
			return _repo.readInt("size2x", 2048);
		}

		public function set size2x(value:int):void
		{
			_repo.writeInt("size2x", value);
		}

		public function save():void
		{
			_repo.flush();
		}
	}
}
