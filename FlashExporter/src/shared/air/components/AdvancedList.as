package shared.air.components
{
	import flash.geom.Point;
	import flash.geom.Rectangle;

	import spark.components.DataGroup;
	import spark.components.List;

	public class AdvancedList extends List
	{
		private var _selectedCaptions:Array = [];

		public function saveSelection():void
		{
			_selectedCaptions = [];

			for each (var item:Object in this.selectedItems)
			{
				_selectedCaptions.push(itemToLabel(item));
			}
		}

		public function restoreSelection():void
		{
			var items:Vector.<Object> = new <Object>[];

			for each (var item:Object in this.dataProvider)
			{
				var label:String = itemToLabel(item);
				if (_selectedCaptions.indexOf(label) >= 0)
					items.push(item);
			}

			selectedItems = items;
			ensureSelectionIsVisible();
		}

		public function restoreSelectionOrSelectFirst():void
		{
			restoreSelection();

			if (!selectedItem)
				selectedIndex = 0;

			ensureSelectionIsVisible();
		}

		public function ensureSelectionIsVisible():void
		{
			validateNow();
			if (selectedIndex >= 0)
				ensureIndexIsVisible(selectedIndex);
		}

		public function get selectedCaptions():Array
		{
			return _selectedCaptions;
		}

		public function set selectedCaptions(value:Array):void
		{
			_selectedCaptions = value;
		}
	}
}
