package actionlib.common.display
{
	import flash.display.DisplayObject;
	import flash.geom.ColorTransform;

	public class ColorTransformUtil
	{
		static public function addColor(target:DisplayObject, color:int):void
		{
			var rgb:Color = new Color(color);
			var transform:ColorTransform = target.transform.colorTransform;
			transform.redOffset += rgb.r;
			transform.greenOffset += rgb.g;
			transform.blueOffset += rgb.b;
			target.transform.colorTransform = transform;
		}

		static public function applyTint(target:DisplayObject, color:int, multiplier:Number = 1.0):void
		{
			var rgb:Color = new Color(color);

			target.transform.colorTransform = new ColorTransform(
					1 - multiplier,
					1 - multiplier,
					1 - multiplier,
					1.0,
					Math.round(rgb.r * multiplier),
					Math.round(rgb.g * multiplier),
					Math.round(rgb.b * multiplier));
		}
	}
}
