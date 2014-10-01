package actionlib.common.errors
{
	public class EnumInstantiationError extends Error
	{
		public function EnumInstantiationError(message:String = "It is not allowed to instantiate enum elements")
		{
			super(message);
		}
	}
}
