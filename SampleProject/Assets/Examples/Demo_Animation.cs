using FlashBundles;
using Flunity;

namespace Examples
{
	public class Demo_Animation : LiveReloadableScene
	{
		protected override void CreateScene ()
		{
			var demo = new McDemo_Animation(stage.root);
			demo.GetChildren<MovieClip>().ForEach(it => it.Play());
		}
	}
}