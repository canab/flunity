package flashexporter.timeline
{
	import actionlib.common.utils.MathUtil;
	import actionlib.common.utils.StringUtil;

	import flash.display.DisplayObject;
	import flash.display.Shape;
	import flash.geom.ColorTransform;
	import flash.geom.Point;

	internal class InstanceParams
	{
		private static var _transformShape:Shape = new Shape();

		public static function serialize(item:InstanceParams):String
		{
			var data:Array = [item.id];

			for (var i:int = 0; i < item.params.length; i++)
			{
				var param:String = (item.params[i] == item.prevParams[i]) ? "" : compact(item.params[i]);
				data.push(param);
			}

			return data.join(",").replace(/,+$/, "");
		}

		private static function compact(value:Number):String
		{
			var stringValue:String = value.toString();

			if (StringUtil.startsWith(stringValue, "0."))
				stringValue = stringValue.substr(1);

			return stringValue
		}

		public var id:int;
		public var params:Array;
		public var prevParams:Array = [];

		public function InstanceParams(id:int, instance:DisplayObject)
		{
			var color:ColorTransform = instance.transform.colorTransform;
			this.id = id;

			_transformShape.transform.matrix = instance.transform.matrix; //clip.scale gives positive values for negative scales

			var rotation:Number = _transformShape.rotation;
			var scale:Point = new Point(_transformShape.scaleX, _transformShape.scaleY);
			if(rotation == 180 && scale.y < 0) //fix flash inconsistency
			{
				scale.x *=-1;
				scale.y *=-1;
                rotation = 0;
			}

			params = [
				toFixed(instance.x, 2),
				toFixed(instance.y, 2),
				toPrecision(rotation * MathUtil.TO_RADIANS, 5),
				toPrecision(scale.x, 5),
				toPrecision(scale.y, 5),
				toPrecision(color.redMultiplier, 3),
				toPrecision(color.greenMultiplier, 3),
				toPrecision(color.blueMultiplier, 3),
				toPrecision(color.alphaMultiplier, 3),
				toPrecision(color.redOffset / 255.0, 3),
				toPrecision(color.greenOffset / 255.0, 3),
				toPrecision(color.blueOffset / 255.0, 3),
				toPrecision(color.alphaOffset / 255.0, 3),
			];
		}

		private function toFixed(value:Number, fraction:int):Number
		{
			return parseFloat(value.toFixed(fraction));
		}

		private function toPrecision(value:Number, digits:int):Number
		{
			return parseFloat(value.toPrecision(digits));
		}

	}
}
