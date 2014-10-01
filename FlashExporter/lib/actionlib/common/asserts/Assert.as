package actionlib.common.asserts
{
	import actionlib.common.errors.NullPointerError;

	public class Assert
	{
		public static function notNull(target:Object, message:String = null):void
		{
			if (target == null)
				throw new NullPointerError(message);
		}
	}
}
