package actionlib.common.display.layouts
{
	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;

	public class GridLayout implements ILayout
	{
		private var _numColumns:int;
		private var _hGridSize:Number;
		private var _vGridSize:Number;
		
		public function GridLayout(hGridSize:Number, vGridSize:Number, numColumns:int)
		{
			_hGridSize = hGridSize;
			_vGridSize = vGridSize;
			_numColumns = numColumns;
		}
		
		public function apply(content:DisplayObjectContainer):void
		{
			var column:int = 0;
			var row:int = 0;
			
			for (var i:int = 0; i < content.numChildren; i++) 
			{
				var child:DisplayObject = content.getChildAt(i);
				child.x = column * _hGridSize;
				child.y = row * _vGridSize;
				
				column++;
				if (column == _numColumns)
				{
					column = 0;
					row++;
				}
				
			}
		}
		
	}

}