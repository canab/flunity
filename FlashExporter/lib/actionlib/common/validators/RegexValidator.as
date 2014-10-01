package actionlib.common.validators
{
	public class RegexValidator implements IValidator
	{
		private var _regexp:RegExp;

		public function RegexValidator(regexp:RegExp)
		{
			_regexp = regexp;
		}

		public function validate(value:*):Boolean
		{
			return _regexp.test(String(value));
		}

		public function get message():String
		{
			return "String does not match: " + String(_regexp);
		}
	}
}
