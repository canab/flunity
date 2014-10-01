using FlashBundles;
using UnityEngine;
using Flunity;
using Flunity.Utils;

namespace Examples
{
	public class Demo_ObjectCreation : LiveReloadableScene
	{
		protected override void CreateScene ()
		{
			stage.root.EnterFrame += it => CreateObject();
		}

		void CreateObject()
		{
			var sprite = SceneBundle.circle_sprite.CreateInstance();
			sprite.color = new Color(RandomUtil.RandomFloat(), RandomUtil.RandomFloat(), RandomUtil.RandomFloat());
			sprite.x = RandomUtil.RandomFloat(0, stage.width);
			sprite.y = RandomUtil.RandomFloat(0, stage.height);
			sprite.RemoveWithAlpha();
			stage.root.AddChild(sprite);
		}
	}
}