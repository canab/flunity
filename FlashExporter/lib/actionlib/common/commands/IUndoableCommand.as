package actionlib.common.commands
{
	public interface IUndoableCommand extends ICommand
	{
		function undo():void;
	}
}