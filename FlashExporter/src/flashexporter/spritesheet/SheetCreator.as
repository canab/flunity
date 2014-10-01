package flashexporter.spritesheet
{
	import actionlib.common.geom.IntSize;

	import flash.display.BitmapData;
	import flash.geom.Point;
	import flash.geom.Rectangle;

	import mx.utils.LinkedList;
	import mx.utils.LinkedListNode;

	import flashexporter.abstracts.AppContext;

	public class SheetCreator extends AppContext
	{
		private const MAX_HEIGHT:int = 8191;

		private static function sizeSorter(frame1:SheetFrame, frame2:SheetFrame):int
		{
			return (frame2.bitmap.height - frame1.bitmap.height);
		}

		private var _frames:LinkedList = new LinkedList();
		private var _sheetSize:IntSize;
		private var _texture:BitmapData;

		private var _expandSize:IntSize = new IntSize();
		private var _putResult:IntSize = new IntSize();
		private var _space:Rectangle = new Rectangle();

		public function SheetCreator(frames:Vector.<SheetFrame>, sheetSize:IntSize)
		{
			frames.sort(sizeSorter);

			for each (var frame:SheetFrame in frames)
			{
				_frames.push(frame);
			}

			_sheetSize = sheetSize;
		}

		public function execute():void
		{
			while(_frames.length > 0)
			{
				if (_texture == null)
				{
					createTexture();
					fillSheet(0, 0, _texture.width, _texture.height);
				}
				else if (_texture.height < MAX_HEIGHT)
				{
					expandTexture();
					var y:int = _space.y + _space.height;
					fillSheet(0, y, _texture.width, _texture.height - y);
				}
				else
				{
					break;
				}
			}

			compactSheet();
		}

		private function createTexture():void
		{
			_texture = SheetUtil.createBitmapData(_sheetSize.width, _sheetSize.height);
			_texture.lock();
		}

		private function expandTexture():void
		{
			app.logger.error("Sprites does not fit in size: {W}x{H}"
					.replace("{W}", _texture.width)
					.replace("{H}", _texture.height));

			var newHeight:Number = Math.min(_texture.height + 1024, MAX_HEIGHT);
			var newTexture:BitmapData = SheetUtil.createBitmapData(
					_texture.width, newHeight);

			newTexture.copyPixels(_texture, _texture.rect, new Point());
			_texture = newTexture;
			_texture.lock();
		}

		private function fillSheet(x:int, y:int, w:int, h:int):void
		{
			_space.x = x;
			_space.y = y;
			_space.width = w;
			_space.height = h;

			if (tryPutFrame(_space, _putResult))
			{
				var rw:int = _putResult.width;
				var rh:int = _putResult.height;

				fillSheet(x + rw, y, w - rw, rh);
				fillSheet(x, y + rh, w, h - rh);
			}
		}

		private function compactSheet():void
		{
			var bounds:Rectangle = SheetUtil.getNonEmptyRect(_texture);
			var height:int = 2;

			while (height < bounds.bottom)
			{
				height *= 2;
			}

			if (height < _sheetSize.height)
			{
				var compacted:BitmapData = SheetUtil.createBitmapData(_texture.width, height);
				compacted.copyPixels(_texture, _texture.rect, new Point());
				_texture = compacted;
			}
		}

		private function tryPutFrame(space:Rectangle, output:IntSize):Boolean
		{
			if (_frames.head == null)
				return false;

			for (var node:LinkedListNode = _frames.head; node.next != null; node = node.next)
			{
				var frame:SheetFrame = SheetFrame(node);

				getExpandSize(frame, _expandSize);

				if (!canBePlacedAt(frame, space, _expandSize))
					continue;

				if (!frame.isEmpty)
				{
					SheetUtil.copyPixels(frame.bitmap, frame.bitmap.rect, _expandSize, _texture, space.topLeft, true);
					frame.sheetX = space.x + _expandSize.width;
					frame.sheetY = space.y + _expandSize.height;
					output.width = frame.width + 2 * _expandSize.width;
					output.height = frame.height + 2 * _expandSize.height;
				}

				_frames.remove(frame);

				return true;
			}

			return false;
		}

		private function canBePlacedAt(frame:SheetFrame, space:Rectangle, expandSize:IntSize):Boolean
		{
			var requiredWidth:int = frame.width + 2 * expandSize.width;
			var requiredHeight:int = frame.height + 2 * expandSize.height;

			return (requiredWidth <= space.width && requiredHeight <= space.height);
		}

		private function getExpandSize(frame:SheetFrame, output:IntSize):void
		{
			if (frame.isEmpty)
			{
				output.width = 0;
				output.height = 0;
			}

			output.width = (frame.width == _sheetSize.width) ? 0 : 1;
			output.height = (frame.height == _sheetSize.height) ? 0 : 1;
		}

		public function get texture():BitmapData
		{
			return _texture;
		}
	}
}







