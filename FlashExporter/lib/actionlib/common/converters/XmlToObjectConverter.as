package actionlib.common.converters
{
	import actionlib.common.interfaces.IConverter;

	public class XmlToObjectConverter implements IConverter
	{
		private var _objectType:Class;
		
		public function XmlToObjectConverter(objectType:Class) 
		{
			_objectType = objectType;
		}
		
		public function convert(value:Object):Object
		{
			var xml:XML = XML(value);
			var result:Object = new _objectType();
			
			for each (var attr:XML in xml.attributes())
			{
				applyProperty(result, attr);
			}
			
			for each (var tag:XML in xml.children())
			{
				applyProperty(result, tag);
			}
			
			return result;
		}

		private function applyProperty(result:Object, value:XML):void
		{
			var attrName:String = String(value.name());

			if (result[attrName] is Boolean)
				result[attrName] = value == "true";
			else
				result[attrName] = value;
		}
	}
}