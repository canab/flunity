package actionlib.common.commands
{
	public class DelayCommand extends CallLaterCommand
	{
		public function DelayCommand(interval:int = 1000)
		{
			super(null, interval);
		}
	}
}
