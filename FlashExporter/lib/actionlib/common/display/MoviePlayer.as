package actionlib.common.display
{
	import actionlib.common.collections.WeakObjectMap;
	import actionlib.common.commands.AsincCommand;
	import actionlib.common.commands.ICancelableCommand;
	import actionlib.common.utils.DisplayUtil;

	import flash.display.MovieClip;
	import flash.events.Event;

	public class MoviePlayer extends AsincCommand implements ICancelableCommand
	{
		static private const _players:WeakObjectMap = new WeakObjectMap(MovieClip, MoviePlayer);

		static public function disposeAllClips():void
		{
			for each (var player:MoviePlayer in _players.getValues())
			{
				player.cancel();
			}
		}

		static public function disposeClip(clip:MovieClip):void
		{
			var player:MoviePlayer = _players[clip];
			if (player)
				player.cancel();
		}

		static public function getPlayer(clip:MovieClip):MovieClip
		{
			return _players[clip];
		}

		/////////////////////////////////////////////////////////////////////////////////////
		//
		// instance
		//
		/////////////////////////////////////////////////////////////////////////////////////

		private var _clip:MovieClip;
		private var _toFrame:int;
		private var _fromFrame:int;
		
		public function MoviePlayer(clip:MovieClip = null, fromFrame:int = 1, toFrame:int = 0)
		{
			_clip = clip;
			_fromFrame = fromFrame;
			_toFrame = (toFrame > 0) ? toFrame : clip.totalFrames;
		}

		public function detachOnComplete():MoviePlayer
		{
			completeEvent.addListener(detachFromDisplay);
			return this;
		}

		private function detachFromDisplay():void
		{
			DisplayUtil.detachFromDisplay(_clip);
		}

		public function playToEnd():void
		{
			play(_clip.currentFrame, _clip.totalFrames);
		}

		public function playToStart():void
		{
			play(_clip.currentFrame, 1);
		}

		public function playTo(toFrame:int):void
		{
			play(_clip.currentFrame, toFrame);
		}

		public function play(fromFrame:int = 1, toFrame:int = 0):void
		{
			_fromFrame = fromFrame;
			_toFrame = (toFrame > 0) ? toFrame : _clip.totalFrames;

			execute();
		}

		override public function execute():void
		{
			var currentPlayer:MoviePlayer = _players[_clip];

			if (currentPlayer)
				currentPlayer.cancel();

			_players[_clip] = this;

			_clip.gotoAndStop(_fromFrame);
			_clip.addEventListener(Event.ENTER_FRAME, onEnterFrame);
		}

		private function onEnterFrame(e:Event):void
		{
			if (_clip.currentFrame == _toFrame)
			{
				stop();
				dispatchComplete();
			}
			else if (_clip.currentFrame < _toFrame)
			{
				_clip.nextFrame();
			}
			else
			{
				_clip.prevFrame();
			}
		}

		public function stop():void
		{
			cancel();
		}

		public function cancel():void
		{
			_clip.removeEventListener(Event.ENTER_FRAME, onEnterFrame);
			_players.removeKey(_clip);
		}

		public function onPlayComplete(handler:Function):MoviePlayer
		{
			return MoviePlayer(onComplete(handler));
		}

		/////////////////////////////////////////////////////////////////////////////////////
		//
		// get/set
		//
		/////////////////////////////////////////////////////////////////////////////////////

		public function get clip():MovieClip
		{
			return _clip;
		}
	}

}