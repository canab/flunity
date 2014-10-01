package flashexporter.data
{
	import actionlib.common.display.Color;
	import actionlib.common.query.conditions.typeIs;
	import actionlib.common.query.from;
	import actionlib.common.utils.MathUtil;
	import actionlib.common.utils.StringUtil;

	import flash.filters.DropShadowFilter;
	import flash.text.TextField;
	import flash.text.TextFormat;

	import flashexporter.ToolApplication;

	public class TextInfo
	{
		public var text:String;
		public var fontName:String;
		public var fontSize:int;
		public var fontColor:Color;

		public var isMultiline:Boolean;
		public var hAlign:String;

		public var isShadow:Boolean;
		public var shadowColor:Color;
		public var shadowX:Number;
		public var shadowY:Number;

		public function TextInfo(field:TextField)
		{
			var format:TextFormat = field.getTextFormat(0);

			text = field.text;
			fontName = ToolApplication.correctFontName(format.font);
			fontSize = format.size ? int(format.size) : 12;
			fontColor = Color.mergeFloatAlpha(field.textColor, field.alpha);
			isMultiline = field.multiline;
			hAlign = format.align;

			var filter:DropShadowFilter = from(field.filters)
					.where(typeIs(DropShadowFilter))
					.findLast();

			isShadow = Boolean(filter);
			if (isShadow)
			{
				shadowColor = Color.mergeFloatAlpha(filter.color, filter.alpha);
				shadowX = Math.round(filter.distance * Math.cos(filter.angle * MathUtil.TO_RADIANS));
				shadowY = Math.round(filter.distance * Math.sin(filter.angle * MathUtil.TO_RADIANS));
			}
		}

		public function get memberType():String
		{
			return isMultiline ? "TextField" : "TextLabel";
		}

	}
}
