package actionlib.common.errors
{
	public class AlreadyInitializedError extends Error
	{
		public function AlreadyInitializedError(message:String = "Object is already initialized")
		{
			super(message);
		}
	}
}