using Flunity.Utils;

namespace Flunity.UserInterface.Controls
{
	public class Slice3HPanel : ControlBase
	{
		private FlashSprite _left;
		private FlashSprite _center;
		private FlashSprite _right;

		public Slice3HPanel(SpriteResource resource)
		{
			construct(resource);
		}

		private void construct(SpriteResource resource)
		{
			_left = new FlashSprite(this, resource);

			_center = new FlashSprite(this, resource);
			_center.x = _left.right;
			_center.currentFrame = 1;

			_right = new FlashSprite(this, resource);
			_right.x = _center.right;

			if (_right.totalFrames >= 3)
				_right.currentFrame = 2;
			else
				_right.flipHorizontal = true;

			width = _left.width + _center.width + _right.width;
			height = MathUtil.Max(_left.height, _center.height, _right.height);
			minSize = size;
		}

		protected override void ApplyLayout()
		{
			_center.width = width - _left.width - _right.width;
			_right.x = _center.right;
		}
	}
}
