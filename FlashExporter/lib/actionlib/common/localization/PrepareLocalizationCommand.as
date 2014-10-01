package actionlib.common.localization
{
	import actionlib.common.commands.AsincMacroCommand;
	import actionlib.common.commands.WaitForEventCommand;

	public class PrepareLocalizationCommand extends AsincMacroCommand
	{
		private var _names:Array /*of String*/;

		public function PrepareLocalizationCommand(names:Array /*of String*/)
		{
			_names = names;
		}

		override public function execute():void
		{
			for each (var bundleId:String in _names)
			{
				var bundle:MessageBundle = LocalizationManager.instance.addBundle(bundleId);
				add(new WaitForEventCommand(bundle.readyEvent));
			}

			super.execute();
		}
	}
}
