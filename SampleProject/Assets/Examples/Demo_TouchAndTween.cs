using FlashBundles;
using UnityEngine;
using Flunity;
using Flunity.Easing;

namespace Examples
{
	public class Demo_TouchAndTween : LiveReloadableScene
	{
		override protected void CreateScene()
		{
			var demo = new McDemo_TouchAndTween(stage.root);
			demo.GetChildren<FlashSprite>().ForEach(ConfigureSprite);
		}

		void ConfigureSprite(FlashSprite sprite)
		{
			var initialPos = sprite.position;

			new TouchListener(sprite).Pressed += it =>
			{
				sprite.Tween(500)
					.Add(DisplayObject.Y, initialPos.y - 200)
					.Add(DisplayObject.SCALE_X, -1)
					.Add(DisplayObject.COLOR, Color.green)
					.Chain(1000)
					.Add(DisplayObject.Y, initialPos.y)
					.Add(DisplayObject.SCALE_X, 1)
					.Add(DisplayObject.COLOR, Color.white)
					.Easing(Elastic.easeOut);
			};
		}
	}
}