package actionlib.common.validators
{
	public class MinLengthValidator implements IValidator
	{
		private var _minLength:int;

		public function MinLengthValidator(minLength:int)
		{
			_minLength = minLength;
		}

		public function validate(value:*):Boolean
		{
			return String(value).length >= _minLength;
		}

		public function get message():String
		{
			return "Minimum length: " + _minLength;
		}
	}
}
