package flashexporter.data
{
	import flash.filesystem.File;

	import shared.air.data.XMLSettings;

	public class UserSettings
	{
		private var _repo:XMLSettings;

		public function UserSettings()
		{
			var file:File = File.userDirectory.resolvePath(".config/FlashExporter/settings.xml");
			_repo = new XMLSettings(file);
		}

		public function get projectPath():String
		{
			return _repo.readString("projectPath");
		}

		public function set projectPath(value:String):void
		{
			_repo.writeString("projectPath", value);
		}

		public function get autoGenerateEnabled():Boolean
		{
			return _repo.readBoolean("autoGenerateEnabled", true);
		}

		public function set autoGenerateEnabled(value:Boolean):void
		{
			_repo.writeBoolean("autoGenerateEnabled", value);
		}

		public function get generateTextures():Boolean
		{
			return _repo.readBoolean("generateTextures", true);
		}

		public function set generateTextures(value:Boolean):void
		{
			_repo.writeBoolean("generateTextures", value);
		}

		public function flush():void
		{
			_repo.flush();
		}
	}
}
