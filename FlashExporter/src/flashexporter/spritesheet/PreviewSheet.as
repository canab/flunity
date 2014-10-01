package flashexporter.spritesheet
{
	import actionlib.common.geom.IntSize;
	import actionlib.common.logging.Logger;

	import flash.display.BitmapData;
	import flash.geom.Point;

	public class PreviewSheet
	{
		private static const DEFAULT_WIDTH:int = 1024;

		private static var _logger:Logger = new Logger(PreviewSheet);
		private var _frames:Vector.<SheetFrame>;
		private var _texture:BitmapData;
		private var _width:int;

		public function PreviewSheet(frames:Vector.<SheetFrame>, width:int = DEFAULT_WIDTH)
		{
			_frames = frames;
			_width = width;
		}

		public function fill():Boolean
		{
			var sheetSize:IntSize = calcSize(1024);

			if (sheetSize.width == 0 || sheetSize.height == 0)
			{
				_logger.error("Size is empty: ", sheetSize.width, sheetSize.height);
				return false;
			}

			if (sheetSize.width > 2048 || sheetSize.height > 2048)
			{
				_logger.error("Size is too large: ", sheetSize.width, sheetSize.height);
				return false;
			}

			createSheet(sheetSize);

			return true;
		}

		private function createSheet(sheetInfo:IntSize):void
		{
			_texture = SheetUtil.createBitmapData(sheetInfo.width, sheetInfo.height);

			var x:int = 0;
			var y:int = 0;
			var rowHeight:int = 0;

			for each (var frame:SheetFrame in _frames)
			{
				if (frame.isEmpty)
					continue;

				var expand:IntSize = getExpandSize(frame);
				var requiredWidth:int = frame.width + 2 * expand.width;
				var requiredHeight:int = frame.height + 2 * expand.height;

				if (x + requiredWidth > sheetInfo.width)
				{
					x = 0;
					y += rowHeight;
					rowHeight = 0;
				}

				SheetUtil.copyPixels(frame.bitmap, frame.bitmap.rect, expand, _texture, new Point(x, y), true);

				x += requiredWidth;
				rowHeight = Math.max(rowHeight, requiredHeight);
			}
		}

		private function calcSize(maxWidth:int):IntSize
		{
			var result:IntSize = new IntSize();

			var x:int = 0;
			var rowWidth:int = 0;
			var rowHeight:int = 0;

			for each (var frame:SheetFrame in _frames)
			{
				if (frame.isEmpty)
					continue;
				
				var expand:IntSize = getExpandSize(frame);
				var requiredWidth:int = frame.width + 2 * expand.width;
				var requiredHeight:int = frame.height + 2 * expand.height;

				if (x + requiredWidth > maxWidth)
				{
					x = 0;

					result.width = Math.max(result.width, rowWidth);
					result.height += rowHeight;

					rowWidth = 0;
					rowHeight = 0;
				}

				x += requiredWidth;
				rowWidth += requiredWidth;
				rowHeight = Math.max(rowHeight, requiredHeight);
			}

			result.width = Math.max(result.width, rowWidth);
			result.height += rowHeight;

			return result;
		}

		private function getExpandSize(frame:SheetFrame):IntSize
		{
			var size:int = (frame.width >= _width) ? 0 : 1;
			return new IntSize(size, size);
		}

		public function get texture():BitmapData
		{
			return _texture;
		}
	}
}

