package actionlib.common.display.layouts
{
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;

	public class HorizontalLayout implements ILayout
	{
		private var _distance:int;
		
		public function HorizontalLayout(distance:Number = -1)
		{
			_distance = distance;
		}
		
		public function apply(content:DisplayObjectContainer):void
		{
			var x:Number = 0;
			
			for (var i:int = 0; i < content.numChildren; i++) 
			{
				var child:DisplayObject = content.getChildAt(i);
				child.x = x;
				x += (_distance > 0) ? _distance : child.width;
			}
		}
		
	}

}