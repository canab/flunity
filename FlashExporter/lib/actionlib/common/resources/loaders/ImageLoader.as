package actionlib.common.resources.loaders
{
	public class ImageLoader extends ContentLoaderBase
	{
		public function ImageLoader(url:String, maxAttempts:int = 3)
		{
			super(url, maxAttempts);
		}
	}
}
