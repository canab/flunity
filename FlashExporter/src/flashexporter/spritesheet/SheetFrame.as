package flashexporter.spritesheet
{
	import flashexporter.rendering.BitmapFrame;

	import flash.display.BitmapData;

	import mx.utils.LinkedListNode;

	public class SheetFrame extends LinkedListNode
	{
		private static function getFrameDescription(x:int, y:int, width:int, height:int, anchorX:int, anchorY:int):XML
		{
			return <frame x={x} y={y} w={width} h={height} ax={anchorX} ay={anchorY}/>;
		}

		public var bitmap:BitmapData;

		public var sheetX:int = 0;
		public var sheetY:int = 0;
		public var anchorX:int = 0;
		public var anchorY:int = 0;

		public function SheetFrame(bitmapFrame:BitmapFrame = null)
		{
			if (bitmapFrame != null && bitmapFrame.data != null)
			{
				bitmap = bitmapFrame.data;
				anchorX = -bitmapFrame.x;
				anchorY = -bitmapFrame.y;
			}
		}

		public function toXml():XML
		{
			return (isEmpty)
				? getFrameDescription(0, 0, 0, 0, 0, 0)
				: getFrameDescription(
					sheetX,
					sheetY,
					bitmap.width,
					bitmap.height,
					anchorX,
					anchorY);
		}

		public function get width():int
		{
			return bitmap ? bitmap.width : 0;
		}

		public function get height():int
		{
			return bitmap ? bitmap.height : 0;
		}

		public function get isEmpty():Boolean
		{
			return width == 0 || height == 0;
		}
	}
}
