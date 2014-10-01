package actionlib.common.reflect
{
	import flash.geom.Point;

	public function castToPoint(object:Object):Point
	{
		return new Point(object.x, object.y);
	}
}
