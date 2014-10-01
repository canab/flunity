package actionlib.common.errors
{
	public class NotInitializedError extends Error
	{
		public function NotInitializedError(message:String = "Object is not initialized")
		{
			super(message);
		}
	}
}