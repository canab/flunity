package actionlib.common.utils
{
	import flash.geom.Point;

	public class MathUtil
	{
		public static var TO_RADIANS:Number = Math.PI / 180;
		public static var TO_DEGREES:Number = 180 / Math.PI;

		public static var PI:Number = Math.PI;
		public static var DOUBLE_PI:Number = 2 * Math.PI;

		public function MathUtil()
		{
			super();
		}
		
		public static function distance2(p1:Point, p2:Point):Number
		{
			var dx:Number = p2.x - p1.x;
			var dy:Number = p2.y - p1.y;
			
			return dx*dx + dy*dy;
		}
		
		public static function claimRange(value:Number, min:Number, max:Number):Number
		{
			if (value < min)
				return min;
			else if (value > max)
				return max;
			else
				return value;
		}
		
		public static function randomInt(minValue:int, maxValue:int):int
		{
			return minValue + int(Math.random() * (maxValue - minValue + 1));
		}

		public static function radiansToDegrees(radians:Number):Number
		{
			return radians * TO_DEGREES;
		}

		public static function degreesToRadians(radians:Number):Number
		{
			return radians * TO_RADIANS;
		}

		/**
		 * returns angle from startPoint to endPoint in ranges [-PI...PI]
		 */
		public static function getAngle(startPoint:Point, endPoint:Point):Number
		{
			var dx:Number = endPoint.x - startPoint.x;
			var dy:Number = endPoint.y - startPoint.y;

			if (dx == 0)
				return (dy > 0) ? 0.5 * PI : -0.5 * PI;
			else
				return Math.atan2(dy,  dx);
		}
		
		/**
		 * Normalize value to range [-PI...PI]
		 * @param	angle
		 * angle value in radians
		 * @return
		 * angle value in range [-PI...PI]
		 */
		public static function normalizeAngle(angle:Number):Number
		{
			angle %= DOUBLE_PI;
			
			if (angle > PI)
				angle -= DOUBLE_PI;
			else if (angle < -PI)
				angle += DOUBLE_PI;
				
			return angle;
		}
		
		/**
		 * Calculates angle difference
		 * @param	angle1
		 * angle value in radians
		 * @param	angle2
		 * angle value in radians
		 * @return
		 * difference in range [-PI...PI]
		 */
		public static function angleDiff(angle1:Number, angle2:Number):Number
		{
			angle1 %= DOUBLE_PI;
			angle2 %= DOUBLE_PI;
			
			if (angle1 < 0)
				angle1 += DOUBLE_PI;
			
			if (angle2 < 0)
				angle2 += DOUBLE_PI;
			
			var diff:Number = angle2 - angle1;
			
			if (diff < -PI)
				diff += DOUBLE_PI;
			
			if (diff > PI)
				diff -= DOUBLE_PI;
			
			return diff;
		}
		
		public static function sign(num:Number):int
		{
			if (num > 0)
				return 1;
			else if (num < 0)
				return -1;
			else
				return 0;
		}
	}
}