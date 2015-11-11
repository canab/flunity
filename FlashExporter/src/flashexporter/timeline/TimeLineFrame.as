package flashexporter.timeline
{
	import actionlib.common.query.from;

	internal class TimeLineFrame
	{
		public var instances:Vector.<InstanceParams>;
		public var labels:Array;

		public function getInstance(id:int):InstanceParams
		{
			for each (var instance:InstanceParams in instances)
			{
				if (instance.id == id)
					return instance;
			}

			return null;
		}

		internal function serialize():String
		{
			var labelsNode:String = labels.join(",");
			var instanceNodes:Array = from(instances).select(InstanceParams.serialize);

			return [labelsNode] + "|" + instanceNodes.join("|");
		}

		public static function serialize(value:TimeLineFrame):String
		{
			return value.serialize();
		}
	}
}
