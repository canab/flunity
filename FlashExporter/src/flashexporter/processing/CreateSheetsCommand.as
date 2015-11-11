package flashexporter.processing
{
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.geom.IntSize;
	import actionlib.common.utils.StringUtil;

	import flash.display.BitmapData;
	import flash.filesystem.File;
	import flash.geom.Rectangle;

	import flashexporter.abstracts.AppContext;
	import flashexporter.data.AppData;
	import flashexporter.data.Swf;
	import flashexporter.data.Symbol;
	import flashexporter.spritesheet.*;

	public class CreateSheetsCommand extends ProcessingCommandBase
	{
		private var _swf:Swf;

		public function CreateSheetsCommand(swf:Swf)
		{
			_swf = swf;
		}

		override protected function onExecute():void
		{
			app.logger.debug("creating sheets...");
			new CallLaterCommand(createSheets).execute();
		}

		private function createSheets():void
		{
			var lowResSize:IntSize = new IntSize(
					_swf.sheetParams.size,
					_swf.sheetParams.size);

			var highResSize:IntSize = new IntSize(
					_swf.sheetParams.size2x,
					_swf.sheetParams.size2x);

			var lowSymbols:Vector.<Symbol> = getSpriteSymbols(false);
			var lowFrames:Vector.<SheetFrame> = getAllFrames(lowSymbols, lowResSize);
			if (!lowFrames)
			{
				fail();
				return;
			}

			if (lowFrames.length == 0)
			{
				dispatchComplete();
				return;
			}

			var lowPacker:SheetCreator = new SheetCreator(lowFrames, lowResSize);
			pack(lowPacker);
			writePack("texture", lowPacker.texture, AppData.getDescription(lowSymbols));

			var highSymbols:Vector.<Symbol> = getSpriteSymbols(true);
			if (highSymbols.length == 0)
			{
				dispatchComplete();
				return;
			}
			else
			{
				highSymbols = mergeResources(lowSymbols, highSymbols);
			}

			var highFrames:Vector.<SheetFrame> = getAllFrames(highSymbols, highResSize);
			if (!highFrames)
			{
				fail();
				return;
			}

			var highPacker:SheetCreator = new SheetCreator(highFrames, highResSize);
			pack(highPacker);
			writePack("texture$hd", highPacker.texture, AppData.getDescription(highSymbols));

			dispatchComplete();
		}

		private function pack(packer:SheetCreator):void
		{
			AppContext.measureTime();
			packer.execute();
			AppContext.traceTime("packing");
		}

		private function getSpriteSymbols(isHd:Boolean):Vector.<Symbol>
		{
			var symbols:Vector.<Symbol> = new <Symbol>[];
			for each (var symbol:Symbol in _swf.symbols)
			{
				if (symbol.frames && symbol.isHd == isHd)
					symbols.push(symbol);
			}
			return  symbols;
		}

		private function getAllFrames(symbols:Vector.<Symbol>, sheetSize:IntSize):Vector.<SheetFrame>
		{
			var frames:Vector.<SheetFrame> = new <SheetFrame>[];

			for each (var symbol:Symbol in symbols)
			{
				for each (var frame:SheetFrame in symbol.frames)
				{
					if (frame.isEmpty)
						continue;

					if (!checkFrameSize(symbol, frame, sheetSize))
						frames = null;

					if (frames)
						frames.push(frame);
				}
			}

			return  frames;
		}

		private function checkFrameSize(symbol:Symbol, frame:SheetFrame, sheetSize:IntSize):Boolean
		{
			var width:int = frame.bitmap.width;
			var height:int = frame.bitmap.height;
			var successful:Boolean = width <= sheetSize.width && height <= sheetSize.height;

			if (!successful)
			{
				var message:String = StringUtil.format("Frame size is too large,\n{0}: {1} x {2}",
						symbol.id, width, height);

				app.logger.error(message);
			}

			return successful;
		}

		private function mergeResources(lowSymbols:Vector.<Symbol>, highSymbols:Vector.<Symbol>)
				:Vector.<Symbol>
		{
			var result:Vector.<Symbol> = new <Symbol>[];

			for each (var low:Symbol in lowSymbols)
			{
				var high:Symbol = findSymbol(highSymbols, low.id + "$hd");
				if (high)
				{
					high.description = low.description;
					result.push(high);
				}
				else
				{
					result.push(low);
				}
			}

			return result;
		}

		private function findSymbol(symbols:Vector.<Symbol>, name:String):Symbol
		{
			for each (var symbol:Symbol in symbols)
			{
				if (symbol.id == name)
					return symbol;
			}
			return null;
		}

		private function writePack(name:String, texture:BitmapData, description:String):void
		{
			var textureFile:File = app.outputDir
					.resolvePath(_swf.bundleName)
					.resolvePath(name);

			AppContext.measureTime();
			app.writeBundleTexture(textureFile, texture);
			AppContext.traceTime("encoding");

			var bounds:Rectangle = SheetUtil.getNonEmptyRect(texture);
			app.logger.info(name, ": ", texture.width, "x", texture.height, "(height: " + bounds.height + ")");


			var descriptionFile:File = app.outputDir
					.resolvePath(_swf.bundleName)
					.resolvePath(name);

			app.writeBundleDescription(descriptionFile, description);
		}

	}

}
