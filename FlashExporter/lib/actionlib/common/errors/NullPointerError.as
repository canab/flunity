package actionlib.common.errors
{
	public class NullPointerError extends Error
	{
		public function NullPointerError(message:String = "Pointer is null") 
		{
			super(message);
		}
	}
}