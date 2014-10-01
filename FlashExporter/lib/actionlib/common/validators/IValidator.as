package actionlib.common.validators
{
	public interface IValidator
	{
		function validate(value:*):Boolean;
		function get message():String;
	}
}
