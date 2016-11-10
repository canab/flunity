using Flunity.Utils;
namespace Flunity.UserInterface.Controls
{
	public class Slice3VPanel : ControlBase
	{
		private FlashSprite _top;
		private FlashSprite _middle;
		private FlashSprite _bottom;

		public Slice3VPanel(SpriteResource resource)
		{
			construct(resource);
		}

		private void construct(SpriteResource resource)
		{
			_top = new FlashSprite(this, resource);

			_middle = new FlashSprite(this, resource);
			_middle.y = _top.bottom;
			_middle.currentFrame = 1;
			
			_bottom = new FlashSprite(this, resource);
			_bottom.y = _middle.bottom;

			if (_bottom.totalFrames >= 3)
				_bottom.currentFrame = 2;
			else
				_bottom.flipVertical = true;

			width = MathUtil.Max(_top.width, _middle.width, _bottom.width);
			height = _top.height + _middle.height + _bottom.height;
			minSize = size;
		}

		protected override void ApplyLayout()
		{
			_middle.height = height - _top.height - _bottom.height;
			_bottom.y = _middle.bottom;
		}
	}
}