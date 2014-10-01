package actionlib.common.commands
{
	import actionlib.common.events.EventSender;

	public interface IAsincCommand extends ICommand
	{
		function get completeEvent():EventSender;
	}
	
}