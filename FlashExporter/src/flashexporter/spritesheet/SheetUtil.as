package flashexporter.spritesheet
{
	import actionlib.common.geom.IntSize;

	import flash.display.BitmapData;
	import flash.filters.BlurFilter;
	import flash.geom.ColorTransform;
	import flash.geom.Point;
	import flash.geom.Rectangle;

	public class SheetUtil
	{
		public static const FILL_COLOR:uint = 0x00000000;

		public static function createBitmapData(width:int, height:int):BitmapData
		{
			return new BitmapData(width, height, true, FILL_COLOR);
		}

		public static function getNonEmptyRect(bitmap:BitmapData):Rectangle
		{
			return bitmap.getColorBoundsRect(0xFF000000, 0x00000000, false);
		}

		public static function copyPixels(source:BitmapData, sourceRect:Rectangle, expandSize:IntSize,
				dest:BitmapData, destPoint:Point, smooth:Boolean = false):IntSize
		{
			if (expandSize.width > 0)
			{
				dest.copyPixels(source,
						new Rectangle(sourceRect.left, sourceRect.top, 1, sourceRect.height),
						new Point(destPoint.x, destPoint.y + expandSize.height));

				dest.copyPixels(source,
						new Rectangle(sourceRect.right - 1, sourceRect.top, 1, sourceRect.height),
						new Point(destPoint.x + sourceRect.width + 1, destPoint.y + expandSize.height));
			}

			if (expandSize.height > 0)
			{
				dest.copyPixels(source,
						new Rectangle(sourceRect.left, sourceRect.top, sourceRect.width, 1),
						new Point(destPoint.x + expandSize.width, destPoint.y));

				dest.copyPixels(source,
						new Rectangle(sourceRect.left, sourceRect.bottom - 1, sourceRect.width, 1),
						new Point(destPoint.x + expandSize.width, destPoint.y + sourceRect.height + 1));
			}

			if (expandSize.width > 0 && expandSize.height > 0)
			{
				dest.setPixel32(
						destPoint.x,
						destPoint.y,
						source.getPixel32(sourceRect.left, sourceRect.top));

				dest.setPixel32(
						destPoint.x + sourceRect.width + 1,
						destPoint.y,
						source.getPixel32(sourceRect.right - 1, sourceRect.top));

				dest.setPixel32(
						destPoint.x,
						destPoint.y + sourceRect.height + 1,
						source.getPixel32(sourceRect.left, sourceRect.bottom - 1));

				dest.setPixel32(
						destPoint.x + sourceRect.width + 1,
						destPoint.y + sourceRect.height + 1,
						source.getPixel32(sourceRect.right - 1, sourceRect.bottom - 1));
			}

			if (smooth)
			{
				var copy:BitmapData = new BitmapData(
						sourceRect.width + 2 * expandSize.width,
						sourceRect.height + 2 * expandSize.height, true, FILL_COLOR);

				copy.copyPixels(source,	sourceRect, new Point(expandSize.width, expandSize.height), null, null, true);
				copy.applyFilter(copy, copy.rect, new Point(), new BlurFilter(2, 2, 1));
				copy.colorTransform(copy.rect, new ColorTransform(1, 1, 1, 0.1, 0, 0, 0, 0));

				dest.copyPixels(copy, copy.rect, new Point(destPoint.x, destPoint.y), null, null, true);

				dest.copyPixels(source,	sourceRect,
						new Point(destPoint.x + expandSize.width, destPoint.y + expandSize.height), null, null, true);
			}
			else
			{
				dest.copyPixels(source,	sourceRect,
						new Point(destPoint.x + expandSize.width, destPoint.y + expandSize.height));
			}


			return new IntSize(
					sourceRect.width + 2 * expandSize.width,
					sourceRect.height + 2 * expandSize.height);
		}
	}
}
