package actionlib.common.utils
{
	import actionlib.common.errors.NullPointerError;

	import flash.external.ExternalInterface;
	import flash.net.URLRequest;
	import flash.net.navigateToURL;

	public class BrowserUtil
	{
		public static const SELF:String = "_self";
		public static const BLANK:String = "_blank";
		public static const PARENT:String = "_parent";
		public static const TOP:String = "_top";

		public static function navigateTop(url:String):void
		{
			navigate(url, TOP)
		}

		public static function navigateBlank(url:String):void
		{
			navigate(url, BLANK)
		}

		public static function navigateParent(url:String):void
		{
			navigate(url, PARENT)
		}

		public static function navigateSelf(url:String):void
		{
			navigate(url, SELF)
		}

		public static function navigate(url:String, window:String):void
		{
			if (ExternalInterface.available)
			{
				try
				{
					var browser:String = ExternalInterface.call(
						"function getBrowser(){return navigator.userAgent}") as String;

					if (browser.indexOf("Firefox") != -1 || browser.indexOf("MSIE 7.0") != -1)
					{
						ExternalInterface.call('window.open("' + url + '","' + window + '")');
					}
					else
					{
					   navigateToURL(new URLRequest(url), window);
					}
				}
				catch (e:Error)
				{
				   navigateToURL(new URLRequest(url), window);
				}
			}
			else
			{
			   navigateToURL(new URLRequest(url), window);
			}
		}

		public static function getURL():String
		{
			var url:String = null;

			try
			{
				url = String(ExternalInterface.call('function() {var afk = document.location.href; return afk;}'));
			}
			catch (e:Error)
			{
				var str:String = e.toString();
				//var str:String = "Error #2060: Security sandbox violation: ExternalInterface caller http://garbuz-studio.com/testprojects/games/gameSpider.swf cannot access file://localhost/E:/active_work/game_spider/bin/gameSpider3.html.";

				var index:int = str.lastIndexOf("://");
				if (index >= 0)
					url = str.substring(index);
			}

			return url;
		}
	}
}