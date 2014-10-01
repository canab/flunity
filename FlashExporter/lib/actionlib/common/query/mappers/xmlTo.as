package actionlib.common.query.mappers
{
	import actionlib.common.converters.XmlToObjectConverter;
	import actionlib.common.interfaces.IConverter;

	public function xmlTo(type:Class):Function
	{
		var converter:IConverter = new XmlToObjectConverter(type);

		return function(item:*):Object
		{
			return converter.convert(item);
		}
	}
}
