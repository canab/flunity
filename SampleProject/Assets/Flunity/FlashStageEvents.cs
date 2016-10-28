using System;

namespace Flunity
{
	public class FlashStageEvents
	{
		#region ObjectAdded

		public event Action<DisplayObject> ObjectAdded;

		internal void DispatchObjectAdded(DisplayObject target)
		{
			if (ObjectAdded != null)
				ObjectAdded.Invoke(target);
		}

		#endregion

		#region ObjectRemoved

		public event Action<DisplayObject> ObjectRemoved;

		internal void DispatchObjectRemoved(DisplayObject target)
		{
			if (ObjectRemoved != null)
				ObjectRemoved.Invoke(target);
		}

		#endregion

		#region FrameLabelEntered

		public event Action<DisplayObject, string> FrameLabelEntered;

		internal void DispatchFrameLabelEntered(DisplayObject target, string label)
		{
			if (FrameLabelEntered != null)
				FrameLabelEntered.Invoke(target, label);
		}

		#endregion
	}
}

