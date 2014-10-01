package actionlib.common.errors
{
	public class IllegalStateError extends Error
	{
		public function IllegalStateError(message:String = "Illegal state error")
		{
			super(message);
		}
	}
}