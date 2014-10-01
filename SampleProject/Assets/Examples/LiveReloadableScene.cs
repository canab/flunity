using UnityEngine;
using FlashBundles;
using Flunity;

namespace Examples
{
	public class LiveReloadableScene : MonoBehaviour
	{
		protected FlashStage stage;

		bool readyFlag = false;

		void OnEnable()
		{
			readyFlag = true;
		}

		void Update()
		{
			// Initialize here for dynamyc scene creation
			// when unity does recompile scripts.
			if (readyFlag)
			{
				readyFlag = false;
				LoadResources();
			}
		}

		void LoadResources()
		{
			SceneBundle.instance.Loaded += it => OnResourcesLoaded();
			SceneBundle.instance.Load();
		}

		void OnResourcesLoaded ()
		{
			stage = GetComponent<FlashStage>();

			// in case when flash resources has been updated in runtime
			// it is necessary remove all previous content
			stage.root.RemoveChildren();

			CreateScene();
		}

		protected virtual void CreateScene()
		{
		}
	}
}

