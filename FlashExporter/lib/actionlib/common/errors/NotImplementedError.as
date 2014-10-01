package actionlib.common.errors
{
	public class NotImplementedError extends Error
	{
		public function NotImplementedError(message:String = "Method is not implemented")
		{
			super(message);
		}
	}
}