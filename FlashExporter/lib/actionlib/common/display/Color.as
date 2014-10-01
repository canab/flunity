package actionlib.common.display
{
	import actionlib.common.utils.MathUtil;

	public class Color
	{
		public static function mergeFloatAlpha(color:uint, alpha:Number):Color
		{
			var intAlpha:uint = Math.round(255 * alpha) << 24;
			return new Color(color | intAlpha);
		}

		public var r:uint = 0;
		public var g:uint = 0;
		public var b:uint = 0;
		public var a:uint = 0;

		public function Color(color:uint = 0)
		{
			value = color;
		}

		public function get value():uint
		{
			return a << 24 | r << 16 | g << 8 | b;
		}

		public function set value(color:uint):void
		{
			a = color >> 24 & 0x0000FF;
			r = color >> 16 & 0x0000FF;
			g = color >> 8 & 0x0000FF;
			b = color & 0x0000FF;
		}

		public function mult(multiplier:Number):Color
		{
			r *= multiplier;
			g *= multiplier;
			b *= multiplier;
			a *= multiplier;

			return this;
		}

		public function add(color:Color):Color
		{
			r = MathUtil.claimRange(r + color.r, 0, 255);
			g = MathUtil.claimRange(g + color.g, 0, 255);
			b = MathUtil.claimRange(b + color.b, 0, 255);
			a = MathUtil.claimRange(a + color.a, 0, 255);

			return this;
		}

		public function toString():String
		{
			return toHexString();
		}

		public function toHexString():String
		{
			return value.toString(16);
		}
	}
}
