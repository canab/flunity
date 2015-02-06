package flashexporter.timeline
{
	import actionlib.common.collections.ObjectMap;
	import actionlib.common.collections.StringMap;
	import actionlib.common.commands.CallLaterCommand;
	import actionlib.common.query.fromDisplay;

	import flash.display.DisplayObject;
	import flash.display.FrameLabel;
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.utils.getQualifiedClassName;

	import flashexporter.abstracts.AsyncAppCommand;
	import flashexporter.data.Swf;
	import flashexporter.data.Symbol;

	public class TimeLineProcessor extends AsyncAppCommand
	{
		public static function create(file:Swf, symbol:Symbol, instance:Object):TimeLineProcessor
		{
			var clip:MovieClip = instance as MovieClip;

			if (!clip)
				return null;

			return symbol.isClass
					? new TimeLineProcessor(file, symbol, clip)
					: null;
		}

		private var _file:Swf;
		private var _symbol:Symbol;
		private var _clip:MovieClip;

		private var _invalidNames:Array = [];
		private var _timeLine:TimeLine = new TimeLine();
		private var _labels:Array;
		private var _instanceIds:StringMap = new StringMap(int);
		private var _currentId:int = 0;

		public function TimeLineProcessor(file:Swf, symbol:Symbol, clip:MovieClip)
		{
			_file = file;
			_symbol = symbol;
			_clip = clip;
		}

		override public function execute():void
		{
			app.logger.debug("timeline: #n (#f frames)"
					.replace("#n", _symbol.id)
					.replace("#f", _clip.totalFrames));

			new CallLaterCommand(process).execute();
		}

		private function process():void
		{
			_timeLine = new TimeLine();
			_labels = _clip.currentLabels;

			processFrames();
			if (appData.isProcessingFailed)
			{
				fail();
				return;
			}
			compactParams();
			updateSymbol();
			dispatchComplete();
		}

		private function processFrames():void
		{
			for (var i:int = 0; i < _clip.totalFrames; i++)
			{
				_timeLine.frames.push(getFrame(i));
				if (appData.isProcessingFailed)
					return;
			}
		}

		private function updateSymbol():void
		{
			_symbol.description = _timeLine.serialize();
			_symbol.children = _timeLine.instances;
			_symbol.isProcessed = true;
		}

		private function compactParams():void
		{
			for (var i:int = 1; i < _timeLine.frames.length; i++)
			{
				var frame:TimeLineFrame = _timeLine.frames[i];
				var prevFrame:TimeLineFrame = _timeLine.frames[i - 1];

				for each (var instance:InstanceParams in frame.instances)
				{
					var prevFrameInstance:InstanceParams = prevFrame.getInstance(instance.id);
					if (prevFrameInstance)
						instance.prevParams = prevFrameInstance.params;
				}
			}
		}

		private function getFrame(frameNum:int):TimeLineFrame
		{
			_clip.gotoAndStop(frameNum + 1);

			var frame:TimeLineFrame = new TimeLineFrame();
			frame.labels = getLabels(frameNum);
			frame.instances = getInstances();
			return frame;
		}

		private function getInstances():Vector.<InstanceParams>
		{
			var addedIds:ObjectMap = new ObjectMap(int, DisplayObject);
			var instanceParams:Vector.<InstanceParams> = new <InstanceParams>[];
			var children:Array = fromDisplay(_clip).select();

			for each (var child:DisplayObject in children)
			{
				var childSymbol:Symbol = new Symbol(child, _file.bundleName);

				if (!checkItem(child, childSymbol))
					continue;

				var instanceId:int = getInstanceId(child);

				if (addedIds.containsKey(instanceId))
				{
					logInvalidChild(child, "Duplicated symbol name: " + child.name);
				}
				else
				{
					addedIds[instanceId] = child;
					instanceParams.push(new InstanceParams(instanceId, child));
				}


				_timeLine.registerInstance(instanceId, childSymbol);
			}

			return instanceParams
		}

		public function getInstanceId(instance:DisplayObject):int
		{
			if (_instanceIds.containsKey(instance.name))
				return _instanceIds[instance.name];

			_instanceIds[instance.name] = _currentId;

			return _currentId++;
		}

		private function getLabels(frameNum:int):Array
		{
			var result:Array = [];
			for each (var frameLabel:FrameLabel in _labels)
			{
				if (frameLabel.frame == frameNum + 1)
					result.push(frameLabel.name);
			}
			return result;
		}

		private function checkItem(child:DisplayObject, childSymbol:Symbol):Boolean
		{
			if (_symbol.isClass && childSymbol.isText)
				return true;

			if (!_symbol.isClass && childSymbol.isText)
			{
				logInvalidChild(child, "MovieClip cannot contain TextField, only class can");
				return false;
			}

			if (!_symbol.isClass && childSymbol.isClass)
			{
				logInvalidChild(child, "MovieClip cannot contain Class, only class can");
				return false;
			}

			var className:String = getQualifiedClassName(child);
			if (className == "flash.display::MovieClip")
			{
				logInvalidChild(child, "Child is not exported");
				return false;
			}

			if (!(child is Sprite))
			{
				logInvalidChild(child, "Child type is not supported: " + className);
				return false;
			}

			return true;
		}

		private function logInvalidChild(child:DisplayObject, message:String):void
		{
			if (_invalidNames.indexOf(child.name) >= 0)
				return;

			_invalidNames.push(child);

            app.logger.error("#m, frame: #f, childNum: #c, pos: (#x, #y)"
		            .replace("#f", _clip.currentFrame)
		            .replace("#c", _clip.getChildIndex(child))
		            .replace("#x", child.x)
		            .replace("#y", child.y)
		            .replace("#m", message));

			appData.isProcessingFailed = true;
		}


	}
}
