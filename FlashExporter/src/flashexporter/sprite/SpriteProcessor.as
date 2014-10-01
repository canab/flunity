package flashexporter.sprite
{
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.display.StageReference;
	import actionlib.common.query.conditions.nameIs;
	import actionlib.common.query.conditions.namePrefixIs;
	import actionlib.common.query.from;
	import actionlib.common.query.fromDisplay;
	import actionlib.common.query.fromDisplayTree;
	import actionlib.common.query.mappers.newObject;
	import actionlib.common.utils.DisplayUtil;
	import flashexporter.rendering.BitmapFrame;
	import flashexporter.rendering.ClipPrerenderer;

	import flash.display.Bitmap;
	import flash.display.BitmapData;
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.display.Sprite;
	import flash.geom.Point;
	import flash.geom.Rectangle;

	import flashexporter.AppConstants;
	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Symbol;
	import flashexporter.spritesheet.SheetFrame;
	import flashexporter.spritesheet.SheetUtil;

	public class SpriteProcessor extends AsyncAppCommand
	{
		public static function create(symbol:Symbol, instance:Object):SpriteProcessor
		{
			var bitmapData:BitmapData = instance as BitmapData;
			if (bitmapData)
				return new SpriteProcessor(symbol, new Bitmap(bitmapData));

			var displayObject:DisplayObject = instance as DisplayObject;
			if (displayObject)
				return new SpriteProcessor(symbol, displayObject);

			return null;
		}

		private var _symbol:Symbol;
		private var _instance:DisplayObject;

		private var _frames:Vector.<BitmapFrame> = new <BitmapFrame>[];
		private var _points:Vector.<DisplayObject>;

		public function SpriteProcessor(symbol:Symbol, instance:DisplayObject)
		{
			_symbol = symbol;
			_instance = instance;
		}

		override public function execute():void
		{
			app.logger.debug("sprite:", _symbol.id);
			new CallLaterCommand(process).execute();
		}

		private function process():void
		{
			var hasBounds:Boolean = hasPredefinedBounds();
			new ClipPrerenderer(wrapInstance(), _frames).renderAllFrames();

			if (_symbol.skipFrames)
				_frames = skipFrames();

			if (!hasBounds)
				removeEmptySpaces();

			_symbol.frames = Vector.<SheetFrame>(from(_frames).select(newObject(SheetFrame)));
			_symbol.description = getDescription();
			_symbol.isProcessed = true;

			dispatchComplete();
		}

		private function wrapInstance():Sprite
		{
			DisplayUtil.setScale(_instance, app.processingScale);

			var container:Sprite = new Sprite();
			container.addChild(_instance);
			container.name = _symbol.id;

			return container;
		}

		private function getDescription():XML
		{
			var description:XML = <sprite/>;

			for each (var pointClip:DisplayObject in _points)
			{
				var container:DisplayObjectContainer = pointClip.parent.parent;
				StageReference.stage.addChild(container);
				var point:Point = DisplayUtil.transformCoords(new Point(), pointClip, container);
				StageReference.stage.removeChild(container);

				var node:XML = <point name={pointClip.name} x={point.x} y={point.y}/>;
				description.appendChild(node)
			}

			return description;
		}

		private function skipFrames():Vector.<BitmapFrame>
		{
			return _frames.filter(
					function myFunction(item:BitmapFrame, index:int, vector:Vector.<BitmapFrame>):Boolean
					{
						return (index % 2 == 0);
					});
		}

		private function hasPredefinedBounds():Boolean
		{
			var container:DisplayObjectContainer = _instance as DisplayObjectContainer;

			return container && fromDisplayTree(container)
					.where(nameIs(ClipPrerenderer.boundsClipName))
					.exists();
		}

		private function removeEmptySpaces():void
		{
			for (var i:int = 0; i < _frames.length; i++)
			{
				var frame:BitmapFrame = _frames[i];
				if (!frame)
					continue;

				var bounds:Rectangle = SheetUtil.getNonEmptyRect(frame.data);
				if (bounds.equals(frame.data.rect))
					continue;

				if (bounds.width == 0 || bounds.height == 0)
				{
					_frames[i] = null;
					continue;
				}

				var condensedBitmap:BitmapData = SheetUtil.createBitmapData(bounds.width, bounds.height);
				condensedBitmap.copyPixels(frame.data, bounds, new Point());
				frame.data = condensedBitmap;
				frame.x += bounds.x;
				frame.y += bounds.y;
			}
		}

	}
}
