package actionlib.common.display
{
	import actionlib.common.errors.NotInitializedError;

	import flash.display.Stage;

	public class StageReference
	{
		private static var _stage:Stage;

		public static function initialize(stage:Stage):void
		{
			_stage = stage;
		}

		public static function get isInitialized():Boolean
		{
			return Boolean(_stage);
		}


		public static function get stage():Stage
		{
			if (!_stage)
				throw new NotInitializedError();
			
			return _stage;
		}
	}
}
