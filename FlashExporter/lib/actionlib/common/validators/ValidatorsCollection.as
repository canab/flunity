package actionlib.common.validators
{
	public class ValidatorsCollection implements IValidator
	{
		private var _validators:Array;
		private var _message:String;

		public function ValidatorsCollection(validators:Array)
		{
			_validators = validators;
		}

		public function validate(value:*):Boolean
		{
			for each (var validator:IValidator in _validators)
			{
				if (!validator.validate(value))
				{
					_message = validator.message;
					return false;
				}
			}

			return true;
		}

		public function get message():String
		{
			return _message;
		}
	}
}
