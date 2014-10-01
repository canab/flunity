package actionlib.common.errors
{
	public class ItemAlreadyExistsError extends Error
	{
		public function ItemAlreadyExistsError(message:String = "Item already exists")
		{
			super(message);
		}
	}
}