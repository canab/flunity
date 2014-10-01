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

		internal function toXML():XML
		{
			var data:Array = from(instances).select(InstanceParams.serialize);
			var xml:XML = <f>{data.join("|")}</f>;

			if (labels.length > 0)
				xml.@labels = labels.join(",");

			return xml;
		}
	}
}
