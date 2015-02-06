package flashexporter.font
{
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.utils.BitmapUtil;
	import actionlib.common.utils.DisplayUtil;

	import flash.display.BitmapData;
	import flash.display.Sprite;
	import flash.geom.Point;
	import flash.geom.Rectangle;
	import flash.text.TextField;
	import flash.text.TextFieldAutoSize;
	import flash.text.TextFormat;

	import flashexporter.ToolApplication;

	import flashexporter.spritesheet.SheetUtil;
	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Symbol;
	import flashexporter.spritesheet.SheetFrame;

	public class FontProcessor extends AsyncAppCommand
	{
		public const SKIP_CHARS:String = "КЕНЗХВАРОСМТуехарос";

		public static function create(symbol:Symbol, instance:Object):FontProcessor
		{
			var sprite:Sprite = instance as Sprite;

			if (sprite == null || sprite.numChildren != 1)
				return null;

			var field:TextField = sprite.getChildAt(0) as TextField;

			if (field == null || field.text.length == 0)
				return null;

			return new FontProcessor(symbol, sprite);
		}

		private var _symbol:Symbol;
		private var _instance:Sprite;

		private var _container:Sprite;
		private var _field:TextField;
		private var _frames:Vector.<FontFrame> = new <FontFrame>[];

		public function FontProcessor(symbol:Symbol, instance:Sprite)
		{
			_symbol = symbol;
			_instance = instance;
		}

		override public function execute():void
		{
			app.logger.debug("font:", _symbol.id);
			new CallLaterCommand(process).execute();
		}

		private function process():void
		{
			initialize();
			renderChars();
			updateSymbol();
			dispatchComplete();
		}

		private function initialize():void
		{
			_container = createContainer();
			_field = TextField(_instance.getChildAt(0));

			var format:TextFormat = _field.getTextFormat(0);
			format.blockIndent = 0;
			format.indent = 0;
			format.leftMargin = 0;
			format.rightMargin = 0;

			_field.wordWrap = false;
			_field.multiline = false;
			_field.defaultTextFormat = format;
			_field.border = false;
			_field.autoSize = TextFieldAutoSize.LEFT;
			_field.x = 0;
			_field.y = 0;
		}

		private function renderChars():void
		{
			var symbols:String = _field.text;

			for (var i:int = 0; i < symbols.length; i++)
			{
				var ch:String = symbols.charAt(i);

				if (ch.charCodeAt(0) < 32 || symbols.indexOf(ch) < i)
					continue;

				if (SKIP_CHARS.indexOf(ch) >= 0)
					continue;

				_frames.push(renderChar(ch));
			}
		}

		private function updateSymbol():void
		{
			_symbol.frames = Vector.<SheetFrame>(_frames);
			_symbol.description = getDescription();
			_symbol.isProcessed = true;
		}

		private function getDescription():String
		{
			var charBounds:Rectangle = _field.getCharBoundaries(0);
			var textFormat:TextFormat = _field.getTextFormat(0);

			var fontName:String = ToolApplication.correctFontName(textFormat.font);
			var fontSize:int = (textFormat.size ? int(textFormat.size) : 12);
			var offsetX:int = int(charBounds.x);
			var offsetY:int = int(charBounds.y);
			var rowHeight:int = int(charBounds.height);
			var letterSpacing:int = int(textFormat.letterSpacing);

			var parts:Array = [fontName, fontSize, offsetX, offsetY, rowHeight, letterSpacing];

			return "font:" + parts.join(",");
		}

		private function renderChar(ch:String):FontFrame
		{
			_field.text = ch;

			var frame:FontFrame = new FontFrame();

			var closeBounds:Rectangle = _field.getCharBoundaries(0);
			frame.symbol = ch;
			frame.symbolWidth = closeBounds.width;

			var redundantBounds:Rectangle = getRedundantBounds(closeBounds);
			var bitmap:BitmapData = BitmapUtil.getBitmapData(_container, redundantBounds);
			var preciseBounds:Rectangle = SheetUtil.getNonEmptyRect(bitmap);

			if (preciseBounds.width > 0 && preciseBounds.height > 0)
			{
				frame.bitmap = SheetUtil.createBitmapData(preciseBounds.width, preciseBounds.height);
				frame.bitmap.copyPixels(bitmap, preciseBounds, new Point());
				frame.offsetX = preciseBounds.x - (closeBounds.x - redundantBounds.x);
				frame.offsetY = preciseBounds.y - (closeBounds.y - redundantBounds.y);
			}

			return frame;
		}

		private function getRedundantBounds(bounds:Rectangle):Rectangle
		{
			return new Rectangle(
					int (bounds.x - 0.5 * bounds.width),
					int (bounds.y - 0.5 * bounds.height),
					int(bounds.width * 2),
					int(bounds.height * 2));
		}

		private function createContainer():Sprite
		{
			DisplayUtil.setScale(_instance, app.processingScale);
			var container:Sprite = new Sprite();
			container.addChild(_instance);
			return container;
		}
	}
}
