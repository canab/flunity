package flashexporter.data
{
	import actionlib.common.utils.StringUtil;

	import flash.display.DisplayObject;

	public class ComponentInfo
	{
		public var className:String;
		public var classArgs:String;
		public var classProps:Object = {};

		public function ComponentInfo(instance:DisplayObject)
		{
			className = instance["className"] || instance["predefinedClass"];
			classArgs = instance["classArgs"] || "";
			var propArray:Array = instance["classProps"] || [];
			while (propArray.length >= 2)
			{
				classProps[propArray.shift()] = propArray.shift();
			}
		}

		public function getProp(propName:String):String
		{
			var value:* = classProps[propName];

			if (value is String)
				return '"' + StringUtil.escape(value) + '"';

			return value;
		}
	}
}
