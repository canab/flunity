package actionlib.common.resources
{
	public class ResourceLock
	{
		private var _name:String;

		public function ResourceLock(name:String)
		{
			_name = name;
		}

		public function toString():String
		{
			return "ResourceLock: " + String(_name);
		}
	}
}
