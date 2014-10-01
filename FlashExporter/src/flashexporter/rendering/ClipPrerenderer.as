package flashexporter.rendering
{
	import actionlib.common.display.StageReference;
	import actionlib.common.processing.IProcessable;
	import actionlib.common.query.conditions.isAnimation;
	import actionlib.common.query.conditions.nameIs;
	import actionlib.common.query.fromDisplayTree;
	import actionlib.common.utils.BitmapUtil;
	import actionlib.common.utils.StringUtil;

	import flash.display.BitmapData;
	import flash.display.DisplayObject;
	import flash.display.MovieClip;
	import flash.display.Sprite;
	import flash.events.Event;
	import flash.geom.Matrix;
	import flash.geom.Rectangle;
	import flash.system.Capabilities;

	public class ClipPrerenderer implements IProcessable
	{
		public static var boundsExpandSize:Number = 0;
		public static var boundsClipName:String = null;
		public static var defaultFillColor:uint = 0x00000000;

		private static var _renderStats:RenderStats = new RenderStats();

		public static function resetStatistics():void
		{
			_renderStats = new RenderStats();
		}

		public static function getTotalStats():String
		{
			return _renderStats.getTotalStats();
		}

		public static function getDetailedStats():String
		{
			return _renderStats.getDetailedStats();
		}

		private static const LABEL_PREFIX:String = "#";
		private static const STATE_REPEAT:String = "#-";
		private static const STATE_NORMAL:String = "#";

		/////////////////////////////////////////////////////////////////////////////////////
		//
		// instance
		//
		/////////////////////////////////////////////////////////////////////////////////////

		private var _frames:Vector.<BitmapFrame>;
		private var _content:Sprite;
		private var _subClips:Array = [];
		private var _totalFrames:int;
		private var _currentFrame:int;
		private var _container:Sprite = new Sprite();
		private var _state:String = STATE_NORMAL;
		private var _stat:StatRecord;

		public function ClipPrerenderer(sprite:Sprite, frames:Vector.<BitmapFrame> = null)
		{
			_content = sprite;
			_frames = frames ? frames : new <BitmapFrame>[];
			_container.addChild(_content);
			_stat = _renderStats.getRecord(String(_content) + " " + _content.name);

			initialize();
		}

		private function initialize():void
		{
			_currentFrame = 1;
			_totalFrames = 1;

			if (isAnimation(_content))
				pushClip(MovieClip(_content));

			fromDisplayTree(_content)
					.where(isAnimation)
					.apply(pushClip);

			_content.addEventListener(Event.ADDED, onAdded);
		}

		private function pushClip(clip:MovieClip):void
		{
			clip.gotoAndStop(1);
			_subClips.push(clip);
			_totalFrames = Math.max(_totalFrames, clip.totalFrames);
			checkStateLabel(clip);
		}

		private function onAdded(event:Event):void
		{
			var clip:MovieClip = event.target as MovieClip;

			if (!clip)
				return;

			if (_subClips.indexOf(clip) == -1)
			{
				_subClips.push(clip);
				clip.gotoAndStop(1);
			}
		}

		public function process():void
		{
			if (!completed)
				_frames.push(renderFrame());

			if (completed)
				_content.removeEventListener(Event.ADDED, onAdded);
			else
				gotoNextFrame();
		}

		public function renderAllFrames():Vector.<BitmapFrame>
		{
			while (!completed)
			{
				process();
			}

			return _frames;
		}

		private function renderFrame():BitmapFrame
		{
			if (_state == STATE_REPEAT)
			{
				_stat.addReusedFrame();

				return (_currentFrame > 1)
					? _frames[_frames.length - 1]
					: null;
			}

			var bounds:Rectangle = getBounds();
			if (bounds.width == 0 || bounds.height == 0)
				return null;

			var frame:BitmapFrame = new BitmapFrame();
			var matrix:Matrix = new Matrix();
			matrix.translate(-bounds.left, -bounds.top);
			frame.x = bounds.left - int(_content.x);
			frame.y = bounds.top - int(_content.y);

			frame.data = new BitmapData(bounds.width, bounds.height, true, defaultFillColor);
			frame.data.draw(_container, matrix);

			_stat.addRenderedFrame(frame);

			return frame;
		}

		private function getBounds():Rectangle
		{
			var boundsClip:DisplayObject = boundsClipName
					? fromDisplayTree(_content).where(nameIs(boundsClipName)).findFirst()
					: null;

			var bounds:Rectangle = null;

			if (boundsClip)
			{
				// air runtime bug
				if (Capabilities.playerType == "Desktop")
				{
					if (StageReference.isInitialized)
					{
						StageReference.stage.addChild(_container);
						bounds = boundsClip.getBounds(_container);
						StageReference.stage.removeChild(_container);
					}
				}

				if (!bounds)
					bounds = boundsClip.getBounds(_container);
			}
			else
			{
				bounds = BitmapUtil.calculateIntBounds(_container);
				bounds.inflate(boundsExpandSize, boundsExpandSize);
			}

			return bounds;
		}

		private function gotoNextFrame():void
		{
			_currentFrame++;
			
			for each (var clip:MovieClip in _subClips)
			{
				if (!_content.contains(clip))
					continue;

				checkStateLabel(clip);

				if (clip.currentFrame < clip.totalFrames)
					clip.nextFrame();
				else
					clip.gotoAndStop(1);
			}
		}

		private function checkStateLabel(clip:MovieClip):void
		{
			var label:String = clip.currentFrameLabel;

			if (!label)
				return;

			if (!StringUtil.startsWith(label, LABEL_PREFIX))
				return;

			_state = label;
		}

		/*///////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		///////////////////////////////////////////////////////////////////////////////////*/

		public function get completed():Boolean
		{
			return _frames.length >= _totalFrames;
		}

		public function get frames():Vector.<BitmapFrame>
		{
			return _frames;
		}
	}

}