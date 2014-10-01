package flashexporter.timeline
{
	import flashexporter.data.Symbol;

	public class TimeLine
	{
		public var resources:Array = [];
		public var instances:Vector.<Symbol> = new <Symbol>[];
		public var frames:Vector.<TimeLineFrame> = new <TimeLineFrame>[];

		public function registerInstance(instanceId:int, symbol:Symbol):void
		{
			if (instances.length > instanceId)
				return;

			var resourceId:int = resources.indexOf(symbol.resourcePath);
			if (resourceId == -1)
			{
				resources.push(symbol.resourcePath);
				resourceId = resources.length - 1;
			}
			symbol.timelineResourceId = resourceId;

			instances.push(symbol);
		}

		public function toXML():XML
		{
			var instanceData:Array = [];
			for each (var instance:Symbol in instances)
			{
				var dataItems:Array = [instance.timelineResourceId, instance.timelineName];
				instanceData.push(dataItems.join(","));
			}

			var framesNode:XML = <frames/>;
			for each (var frame:TimeLineFrame in frames)
			{
				framesNode.appendChild(frame.toXML());
			}

			var xml:XML = <timeline/>;
			xml.resources = resources.join(",");
			xml.instances = instanceData.join("|");
			xml.appendChild(framesNode);

			return xml;
		}
	}
}
