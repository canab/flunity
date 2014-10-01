package actionlib.common.processing
{
	public interface IProcessable
	{
		function process():void;
		function get completed():Boolean;
	}
}