using System;
using System.Collections.Generic;
using System.Linq;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Container that represents animated timeline created in Adobe Flash
	/// </summary>
	public class MovieClip : DisplayContainer
	{
		public bool nestedAnimationEnabled;

		private TimeLine _timeLine;
		private MovieClipResource _resource;
		private Dictionary<int, Action> _frameActions;
		private DisplayObject[] _instances;

		public MovieClip(MovieClipResource resource)
		{
			Initialize(resource);
		}

		private void Initialize(MovieClipResource resource)
		{
			_resource = resource;
			_timeLine = _resource.timeLine;

			_instances = FlashResources.isReloadingPerformed
				? ConstructFromResource()
				: ConstructInstances();

			totalFrames = _timeLine.frames.Length;

			OnFrameChange();
		}

		protected virtual DisplayObject[] ConstructInstances()
		{
			return ConstructFromResource();
		}

		protected DisplayObject[] ConstructFromResource()
		{
			var count = _timeLine.instances.Length;

			var instances = new DisplayObject[count];

			for (int i = 0; i < count; i++)
			{
				var instName = _timeLine.instances[i].name;
				var resourcePath = _timeLine.GetResourcePath(i);
				var resource = FlashResources.GetResource<IDisplayResource>(resourcePath);

				DisplayObject instance;

				if (resource != null)
				{
					instance = resource.CreateInstance();
				}
				else if (resourcePath == "flash/text/TextField")
				{
					instance = new TextLabel() { text = ":-)" };
				}
				else
				{
					var className = resourcePath
						.Replace("Placeholders/", "")
						.Replace("/", ".");

					var type = Type.GetType(className);
					if (type != null)
						instance = (DisplayObject) Activator.CreateInstance(type);
					else
						throw new Exception("Resource not found: " + resourcePath);
				}

				instance.name = instName;
				instances[i] = instance;

				var field = GetType().GetField(instName);
				if (field != null && field.FieldType.IsInstanceOfType(instance))
					field.SetValue(this, instance);
			}

			return instances;
		}

		protected override void ResetDisplayObject()
		{
			if (_frameActions != null)
				_frameActions.Clear();

			nestedAnimationEnabled = true;
			base.ResetDisplayObject();
		}

		/// <summary>
		/// Goes to the frames with specified label.
		/// </summary>
		public MovieClip GotoLabel(string label)
		{
			var frameNum = GetFrameNum(label);
			if (frameNum >= 0)
				currentFrame = frameNum;
			return this;
		}

		/// <summary>
		/// Returns all labels in the current frame. No objects are allocated.
		/// </summary>
		public string[] CurrentLabels()
		{
			return _timeLine.frames[currentFrame].labels;
		}

		/// <summary>
		/// Returns labels from all frames. Allocates returning collection.
		/// </summary>
		public List<FrameLabel> GetAllLabels()
		{
			var labels = new List<FrameLabel>();

			for (int i = 0; i < totalFrames; i++)
			{
				foreach (var label in _timeLine.frames[i].labels)
				{
					labels.Add(new FrameLabel { frame = i, name = label});
				}
			}

			return labels;
		}

		/// <summary>
		/// Returns all labels in the specified frame. No objects are allocated.
		/// </summary>
		public string[] GetFrameLabels(int frameNum)
		{
			return (frameNum < 0 || frameNum >= totalFrames)
				? new string[] { }
				: _timeLine.frames[frameNum].labels;
		}

		/// <summary>
        /// Assigns an action which will be invoked when <c>MovieClip</c> goes to the specified frame.
		/// </summary>
		public void SetFrameAction(int frameNum, Action action)
		{
			if (_frameActions == null)
				_frameActions = new Dictionary<int, Action>();

			_frameActions[frameNum] = action;
		}

		/// <summary>
        /// Assigns an action which will be invoked when <c>MovieClip</c> goes to the frame
		/// which has specified label.
		/// </summary>
		public void SetLabelAction(string labelName, Action action)
		{
			for (int i = 0; i < totalFrames; i++)
			{
				if (_timeLine.frames[i].labels.Contains(labelName))
					SetFrameAction(i, action);
			}
		}

		/// <summary>
		/// Returns frame which has specified label (-1 if not found)
		/// </summary>
		public int GetFrameNum(string labelName)
		{
			for (int i = 0; i < totalFrames; i++)
			{
				if (_timeLine.frames[i].labels.Contains(labelName))
					return i;
			}
			return -1;
		}

		protected override void OnFrameChange()
		{
			UpdateInstances();

			if (_frameActions != null)
			{
				Action frameAction;
				if (_frameActions.TryGetValue(currentFrame, out frameAction))
					frameAction.Invoke();
			}

			var labels = CurrentLabels();

			for (int i = 0; i < labels.Length; i++)
			{
				if (stage != null)
					stage.events.DispatchFrameLabelEntered(this, labels[i]);
			}
		}

		private void UpdateInstances()
		{
			var frame = _timeLine.frames[currentFrame];
			var itemNode = numChildren > 0 ? GetChildAt(0).node : null;
			var instanceIndex = 0;
			var instanceCount = frame.instances.Length;

			while (itemNode != null && instanceIndex < instanceCount)
			{
				var item = itemNode.Value;
				var instance = frame.instances[instanceIndex];

				if (item.timelineInstanceId == instance.id)
				{
					instance.ApplyPropertiesTo(item, anchor);

					if (nestedAnimationEnabled && item.totalFrames > 1)
						item.StepForward();

					instanceIndex++;
					itemNode = itemNode.Next;
				}
				else if (frame.HasInstance(item.timelineInstanceId))
				{
					var newItem = GetChildItem(instance.id);
					instance.ApplyPropertiesTo(newItem, anchor);
					AddChildBefore(item, newItem);
					instanceIndex++;
				}
				else
				{
					var nextNode = itemNode.Next;
					if (item.timelineInstanceId >= 0)
						RemoveChild(item);
					itemNode = nextNode;
				}
			}

			while (instanceIndex < instanceCount)
			{
				var instance = frame.instances[instanceIndex];
				var newItem = GetChildItem(instance.id);
				instance.ApplyPropertiesTo(newItem, anchor);
				AddChild(newItem);
				instanceIndex++;
			}

			while (itemNode != null)
			{
				var nextNode = itemNode.Next;
				var item = itemNode.Value;
				if (item.timelineInstanceId >= 0)
					RemoveChild(item);
				itemNode = nextNode;
			}
		}

		private DisplayObject GetChildItem(int instanceId)
		{
			DisplayObject displayObject = _instances[instanceId];
			displayObject.currentFrame = 0;
			displayObject.timelineInstanceId = instanceId;
			return displayObject;
		}
	}
}
