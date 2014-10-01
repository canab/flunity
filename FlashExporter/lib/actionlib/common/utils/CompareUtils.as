package actionlib.common.utils
{
	public class CompareUtils
	{
		public static function compareStrings(a:String, b:String):int
		{
			if (a > b)
				return 1;
			else if (a < b)
				return -1;
			else
				return 0;
		}
		
		public static function compareNumbers(a:Number, b:Number):int
		{
			if (a > b)
				return 1;
			else if (a < b)
				return -1;
			else
				return 0;
		}

		public static function compareDates(a:Date, b:Date):int
		{
			if (a > b)
				return 1;
			else if (a < b)
				return -1;
			else
				return 0;
		}

	}
}
