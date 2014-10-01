package flashexporter.rendering
{
	import actionlib.common.collections.StringMap;
	import actionlib.common.query.from;

	internal class RenderStats
	{
		private var _records:StringMap = new StringMap(StatRecord);

		public function getRecord(key:String):StatRecord
		{
			var record:StatRecord = _records[key];

			if (!record)
				_records[key] = (record = new StatRecord(key));

			return record;
		}

		public function getDetailedStats():String
		{
			var stats:Array = _records.getValues();
			stats.sortOn("totalBytes", Array.NUMERIC | Array.DESCENDING);
			stats.push(getTotalStats());
			var result:String = stats.join("\n");
			return result;
		}

		public function getTotalStats():String
		{
			var result:StatRecord = new StatRecord("total");
			from(_records).apply(result.add);

			return result.toString();
		}
	}
}
