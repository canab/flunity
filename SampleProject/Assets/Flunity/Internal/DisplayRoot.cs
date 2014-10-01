using Flunity;

namespace Flunity.Internal
{
	/// <summary>
	/// Root container for FlashStage
	/// </summary>
	internal class DisplayRoot : DisplayContainer
	{
		private readonly DisplayTreeRenderer _stageRenderer;

		public DisplayRoot(FlashStage component)
		{
			stage = component;
			name = "root";
			drawOptions = DrawOptions.DEFAULT;

			_stageRenderer = new DisplayTreeRenderer(stage);
		}

		public override void Draw()
		{
			_stageRenderer.DrawStage();
		}
	}
}
