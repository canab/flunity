package actionlib.common.errors
{
	public class ItemNotFoundError extends Error
	{
		public function ItemNotFoundError(message:String = "Item not found")
		{
			super(message);
		}
	}
}