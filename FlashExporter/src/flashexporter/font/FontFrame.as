package flashexporter.font
{
	import flashexporter.spritesheet.SheetFrame;

	public class FontFrame extends SheetFrame
	{
		public var symbol:String;
		public var symbolWidth:int;
		public var offsetX:int;
		public var offsetY:int;

		public function FontFrame()
		{
			super();
		}

		override public function serialize():String
		{
			return [super.serialize(), symbolWidth, offsetX, offsetY, symbol].join(",");
		}
	}
}
