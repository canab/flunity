package actionlib.common.localization
{
	import actionlib.common.interfaces.IConverter;

	internal class DefaultMessageConverter implements IConverter
	{
		public function convert(value:Object):Object
		{
			var result:Object = {};
			var xml:XML = new XML(String(value));

			for each (var node:XML in xml.m)
			{
				result[node.@id] = String(node)
					.replace(/\r\n/g, "\n")
					.replace(/\|/g, "\n");
			}

			return result;
		}
	}
}
