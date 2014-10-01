package flashexporter.rendering
{
	import flash.display.BitmapData;

	public class BitmapFrame
	{
		public var x:int = 0;
		public var y:int = 0;
		public var data:BitmapData;

		public function BitmapFrame(data:BitmapData = null)
		{
			this.data = data;
		}

		public function get dataSize():int
		{
			return data.width * data.height * (data.transparent ? 4 : 3);
		}
	}
}