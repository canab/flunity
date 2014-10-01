package flashexporter.font
{
	import flashexporter.rendering.BitmapFrame;

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

		override public function toXml():XML
		{
			var description:XML = super.toXml();
			description.@s = symbol;
			description.@sw = symbolWidth;
			description.@ox = offsetX;
			description.@oy = offsetY;

			return description;
		}
	}
}
