package flashexporter.processing
{
	import actionlib.common.commands.IAsincCommand;
	import actionlib.common.display.Color;
	import actionlib.common.query.from;
	import actionlib.common.utils.ArrayUtil;
	import actionlib.common.utils.StringUtil;

	import flash.filesystem.File;
	import flash.text.TextFormatAlign;

	import flashexporter.data.Swf;
	import flashexporter.data.Symbol;
	import flashexporter.data.TextInfo;

	import shared.air.utils.FileUtil;

	public class GenerateSourceCommand extends ProcessingCommandBase implements IAsincCommand
	{
		[Embed(source = "../../../templates/bundle.cs.txt", mimeType="application/octet-stream")]
		private static var csharp_bundle:Class;

		[Embed(source = "../../../templates/class.cs.txt", mimeType="application/octet-stream")]
		private static var csharp_class:Class;

		private var _using:Array = [];
		private var _swf:Swf;

		public function GenerateSourceCommand(swf:Swf)
		{
			_swf = swf;
		}

		override protected function onExecute():void
		{
			addUsing("Flunity");

			var resources:Array = from(_swf.symbols)
					.where(function(it:Symbol):Boolean {
						return !it.isHd	})
					.select(getResource);

			var classes:Array = from(_swf.symbols)
					.where(Symbol.isClassProp)
					.select(getClass);

			var bundleSource:String = String(new csharp_bundle())
					.replace("#using", _using.join("\n"))
					.replace("#namespace", "FlashBundles")
					.replace(/#className/g, _swf.bundleName)
					.replace("#resources", resources.join("\n"))
					.replace("#classes", classes.join("\n\n"));

			var file:File = app.outputDir.resolvePath(_swf.bundleName + ".cs");

			FileUtil.writeText(file, FileUtil.replaceNativeEol(bundleSource));
			dispatchCompleteAsync();
		}

		private function getResource(symbol:Symbol):String
		{
			const TEMPLATE:String = '\t\tpublic static readonly #type #name = new #type("#path");';

			return TEMPLATE
				.replace(/#type/g, symbol.resourceType)
				.replace("#name", symbol.name)
				.replace("#path", symbol.resourcePath);
		}

		private function getClass(symbol:Symbol):String
		{
			var fields:Array = [];
			var constructs:Array = [];
			var uniqueNames:Object = {};
			var arrayIndex:int = 0;

			for each (var child:Symbol in symbol.children)
			{
				var memberName:String = child.timelineName;
				var template:String;
				var field:String;

				if (!symbol.isExtend && memberName && !(memberName in uniqueNames))
				{
					uniqueNames[memberName] = memberName;

					field = "\t\tpublic #type #name;"
							.replace(/#type/g, child.memberType)
							.replace("#name", memberName);
					fields.push(field);
				}

				var construct:String;
				if (child.isText)
				{
					var textInfo:TextInfo = child.textInfo;
					template = memberName
						? '\t\t\tinstances[#i] = #name = new #type("#fontName", #fontSize)'
						: '\t\t\tinstances[#i] = new #type("#fontName", #fontSize)';
					construct = template
							.replace("#i", arrayIndex++)
							.replace("#name", memberName)
							.replace("#type", textInfo.memberType)
							.replace("#fontName", textInfo.fontName)
							.replace("#fontSize", textInfo.fontSize);
					constructs.push(construct);
					constructs.push("\t\t\t{");
					if (memberName)
						constructs.push('\t\t\t\tname = "#",'.replace("#", memberName));
					constructs.push('\t\t\t\ttext = "#",'.replace("#", StringUtil.escape(textInfo.text)));
					constructs.push('\t\t\t\ttextColor = #,'.replace("#", getColor(textInfo.fontColor)));
					constructs.push('\t\t\t\thAlignment = #,'.replace("#", getHAlign(textInfo.hAlign)));
					constructs.push('\t\t\t\tsize = #,'.replace("#", getVector(child.timelineWidth, child.timelineHeight)));
					if (textInfo.isShadow)
					{
						constructs.push('\t\t\t\tshadowColor = #,'.replace("#", getColor(textInfo.shadowColor)));
						constructs.push('\t\t\t\tshadowOffset = #,'.replace("#", getVector(textInfo.shadowX, textInfo.shadowY)));
					}
					constructs.push("\t\t\t};");
				}
				else if (child.isPlaceHolder)
				{
					template = memberName
						? '\t\t\tinstances[#i] = #name = new #type()'
						: '\t\t\tinstances[#i] = new #type()';
					construct = template
							.replace("#i", arrayIndex++)
							.replace("#name", memberName)
							.replace("#type", child.memberType);
					constructs.push(construct);
					constructs.push("\t\t\t{");
					if (memberName)
						constructs.push('\t\t\t\tname = "#",'.replace("#", memberName));
					constructs.push('\t\t\t\tsize = #,'.replace("#", getVector(child.timelineWidth, child.timelineHeight)));
					constructs.push('\t\t\t\tanchor = #,'.replace("#", getVector(child.timelineAx, child.timelineAy)));
					constructs.push("\t\t\t};");
				}
				else
				{
					var defaultTemplate:String = memberName
							? '\t\t\tinstances[#i] = #name = new #type(#resource) {name = "#name"};'
							: '\t\t\tinstances[#i] = new #type(#resource);';
					construct = defaultTemplate
							.replace("#i", arrayIndex++)
							.replace(/#name/g, memberName)
							.replace("#type", child.memberType)
							.replace("#resource", child.generatedResource);
					constructs.push(construct);
				}
			}

			if (fields.length > 0)
				fields = [""].concat(fields).concat("");

			return String(new csharp_class())
					.replace("#using", _using.join("\n"))
					.replace(/#className/g, symbol.memberType)
					.replace("#baseClass", symbol.baseType)
					.replace("#resource", symbol.bundleName + "." + symbol.name)
					.replace("#fields", fields.join("\n"))
					.replace("#numChildren", symbol.children.length)
					.replace("#construct", constructs.join("\n"));
		}

		private function getVector(x:Number, y:Number):String
		{
			addUsing("UnityEngine");
			return "new Vector2(#xf, #yf)"
					.replace("#x", x)
					.replace("#y", y);
		}

		private function getHAlign(hAlign:String):String
		{
			switch (hAlign)
			{
				case TextFormatAlign.CENTER:
					return "HAlign.CENTER";
				case TextFormatAlign.LEFT:
					return "HAlign.LEFT";
				case TextFormatAlign.RIGHT:
					return "HAlign.RIGHT"
			}

			return "HAlign.LEFT";
		}

		private function getColor(color:Color):String
		{
			addUsing("UnityEngine");
			return "new Color32(#r, #g, #b, #a)"
					.replace("#r", color.r)
					.replace("#g", color.g)
					.replace("#b", color.b)
					.replace("#a", color.a)
		}

		private function addUsing(item:String):void
		{
			var using:String = "using " + item + ";";
			ArrayUtil.pushUniqueItem(_using, using);
		}
	}
}
