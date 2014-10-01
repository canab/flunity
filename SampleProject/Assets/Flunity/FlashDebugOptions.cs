using UnityEngine;
using Flunity.Common;
using Flunity;

namespace Flunity
{
	[ExecuteInEditMode]
	public class FlashDebugOptions : MonoBehaviour
	{
		public bool drawHitAreas = DebugDraw.drawHitAreas;

		public bool drawPlaceholders = DebugDraw.drawPlaceholders;

		public bool drawTextFields = DebugDraw.drawTextField;

		public bool liveReloadingEnabled = FlashResources.isReloadingEnabled;

		public LogLevel logLevel = FlashResources.logLevel;

		void Update()
		{
			if (Debug.isDebugBuild)
			{
				DebugDraw.drawHitAreas = drawHitAreas;
				DebugDraw.drawTextField = drawTextFields;
				DebugDraw.drawPlaceholders = drawPlaceholders;
				FlashResources.logLevel = logLevel;
				FlashResources.isReloadingEnabled = liveReloadingEnabled;
			}
		}
	}
}

