using System;
using System.Collections;
using UnityEngine;

namespace Flunity.Internal
{
	internal struct InstanceInfo
	{
		public string name;
		public int resourceNum;
	}

	/// <summary>
	/// Holds exported from Flash timeline data
	/// </summary>
	internal class TimeLine
	{
		internal static readonly TimeLine EMPTY = new TimeLine()
		{
			resources = new string[0],
			instances = new InstanceInfo[0],
			frames = new[]
			{
				new FrameData()
				{
					labels = new string[0],
					instances = new InstanceData[] {},
					existingInstancesBits = new BitArray(0),
				}
			}
		};
		
		internal string[] resources;
		internal InstanceInfo[] instances;
		internal FrameData[] frames;

		internal string GetResourcePath(int instanceId)
		{
			var resourceNum = instances[instanceId].resourceNum;
			return resources[resourceNum];
		}
	}

	internal class FrameData
	{
		public string[] labels;
		public InstanceData[] instances;
		public BitArray existingInstancesBits;

		public bool HasInstance(int id)
		{
			return id >= 0
				&& id < existingInstancesBits.Count
				&& existingInstancesBits[id];
		}
	}

	internal class InstanceData
	{
		public int id;
		public Vector2 position;
		public Vector2 scale;
		public float rotation;
		public ColorTransform color;

		internal InstanceData Clone()
		{
			return new InstanceData
			{
				id = id,
				position = position,
				rotation = rotation,
				scale = scale,
				color = color,
			};
		}

		public void ApplyPropertiesTo(DisplayObject target, Vector2 anchor)
		{
			target.position = position + anchor;
			target.rotation = rotation;
			target.scale = scale;
			target.colorTransform = color;
		}
	}

}