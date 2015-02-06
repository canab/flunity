package flashexporter.font
{
	import flashexporter.spritesheet.SheetFrame;

	public class FontFrame extends SheetFrame
	{
		public var symbol:String;
		public var symbolWidth:int;

		public function FontFrame()
		{
			super();
		}

		override public function serialize():String
		{
			return [super.serialize(), symbolWidth, symbol].join(",");
		}
	}
}
