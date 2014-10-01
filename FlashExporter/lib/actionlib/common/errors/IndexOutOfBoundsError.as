package actionlib.common.errors
{
	public class IndexOutOfBoundsError extends Error
	{
		public function IndexOutOfBoundsError(message:String = "Index is out of bounds")
		{
			super(message);
		}
	}
}