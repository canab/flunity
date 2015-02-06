package flashexporter.data
{
	import actionlib.common.utils.StringUtil;

	import flash.display.DisplayObject;
	import flash.display.DisplayObjectContainer;
	import flash.geom.Rectangle;
	import flash.text.TextField;
	import flash.utils.getQualifiedClassName;

	import flashexporter.AppConstants;
	import flashexporter.spritesheet.SheetFrame;

	public class Symbol
	{
		public static function isTextureProp(it:Symbol):Boolean { return it.isTexture}
		public static function isSpriteProp(it:Symbol):Boolean { return it.isSprite}
		public static function isClassProp(it:Symbol):Boolean { return it.isClass; }
		public static function isHdProp(it:Symbol):Boolean { return it.isHd; }
		public static function bundleNameProp(it:Symbol):String { return it.bundleName;	}
		public static function inBundle(bundleName:String):Function
		{
			return function(symbol:Symbol):Boolean
			{
				return symbol.bundleName == bundleName;
			}
		}

		public var id:String;
		public var isHd:Boolean;
		public var isClass:Boolean;
		public var isFont:Boolean = false;
		public var isText:Boolean = false;
		public var isTexture:Boolean = false;
		public var skipFrames:Boolean;
		public var isExtend:Boolean = false;
		public var isPlaceHolder:Boolean = false;

		public var name:String;
		public var bundleName:String;
		public var resourcePath:String;
		public var memberType:String;
		public var baseType:String;
		public var resourceType:String;
		public var generatedResource:String = "";

		public var isProcessed:Boolean = false;
		public var description:String;
		public var frames:Vector.<SheetFrame>;
		public var children:Vector.<Symbol>;

		public var timelineName:String;
		public var timelineResourceId:int;
		public var timelineWidth:int;
		public var timelineHeight:int;
		public var timelineAx:int;
		public var timelineAy:int;
		public var textInfo:TextInfo;
		public var compInfo:ComponentInfo;

		private var _hints:Array;

		public function Symbol(source:Object, specifiedBundleName:String = null)
		{
			if (source is String)
				initializeWithQualifiedName(source as String, specifiedBundleName);
			else if (source is DisplayObject)
				initializeWithInstance(source as DisplayObject, specifiedBundleName)
		}

		private function initializeWithInstance(instance:DisplayObject, specifiedBundleName:String):void
		{
			isText = instance is TextField;

			initializeWithQualifiedName(getQualifiedClassName(instance), specifiedBundleName);

			if (instance.name.indexOf("instance") != 0 && instance.name.indexOf("__id") != 0)
				timelineName = instance.name;

			var boundsObject:DisplayObject = (instance is DisplayObjectContainer)
					? (instance as DisplayObjectContainer).getChildByName(AppConstants.BOUNDS_NAME)
					: null;
			var timelineBounds:Rectangle = boundsObject
					? boundsObject.getBounds(instance)
					: instance.getBounds(instance);

			timelineWidth = timelineBounds.width;
			timelineHeight = timelineBounds.height;
			timelineAx = -timelineBounds.x;
			timelineAy = -timelineBounds.y;

			if (isText)
			{
				textInfo = new TextInfo(TextField(instance));
				memberType = textInfo.memberType;
			}
		}

		public function initializeWithQualifiedName(qualifiedName:String, specifiedBundleName:String = null):void
		{
			id = qualifiedName.replace("::", ".");

			_hints = id.split("$");

			var pathParts:Array = id.split("$")[0].split(".");
			name = pathParts[pathParts.length - 1];
			bundleName = pathParts.length > 1
					? pathParts[0]
					: specifiedBundleName;
			resourcePath = pathParts.length > 1
					? pathParts.join("/")
					: bundleName + '/' + pathParts[0];

			isPlaceHolder = (bundleName == "Placeholders");
			if (isPlaceHolder)
			{
				timelineName = StringUtil.lowerFirstChar(name);
				memberType = pathParts.join(".")
					.replace(bundleName + ".", "");

				var defaultNamespace:String = "ActionLib.Display.";
				if (StringUtil.startsWith(memberType, defaultNamespace))
					memberType = memberType.replace(defaultNamespace, "");

				return;
			}

			isClass = name.charAt(0) != "_"
					&& name.charAt(0) == name.charAt(0).toUpperCase();

			isExtend = hasHint("ext");
			isHd = hasHint("hd");
			isFont = hasHint("font");
			skipFrames = hasHint("skipFrames");

			if (isClass)
			{
				memberType = StringUtil.upperFirstChar(name);
				resourceType = "MovieClipResource";
				baseType = "MovieClip";
			}
			else if (isFont)
			{
				memberType = "";
				resourceType = "FontResource";
				isTexture = true;
			}
			else
			{
				memberType = "FlashSprite";
				resourceType = "SpriteResource";
				isTexture = true;
				generatedResource = bundleName + "." + name;
			}
		}

		private function hasHint(hint:String):Boolean
		{
			return _hints.indexOf(hint) > 0;
		}

		public function getDescription():String
		{
			var result:Array = [resourcePath];

			if (isHd)
				result.push("hd:");

			if (description && description.length > 0)
				result.push(description);

			if (frames)
			{
				for each (var frame:SheetFrame in frames)
				{
					result.push(frame.serialize());
				}
			}

			return result.join("\n");
		}

		public function get isSprite():Boolean
		{
			return isTexture && !isFont;
		}

		public function get fullMemberName():String
		{
			return bundleName + "." + name;
		}

		public function toString():String
		{
			return "Symbol[" + id + "]";
		}
	}
}
