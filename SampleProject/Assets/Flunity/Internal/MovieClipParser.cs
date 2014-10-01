using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Flunity;

namespace Flunity.Internal
{
	internal class MovieClipParser
    {
		public static TimeLine ReadTimeLine(XContainer xml)
        {
            var timeLine = new TimeLine();

            timeLine.resources = xml.Element("resources").Value.Split(',');
            timeLine.instances = ReadInstanceInfo(xml);
            timeLine.frames = ReadFrames(xml, timeLine.instances.Length);

            return timeLine;
        }

        private static InstanceInfo[] ReadInstanceInfo(XContainer xml)
        {
            string[] instanceStrings = xml.Element("instances").Value.Split('|');
	        var instanceCount = instanceStrings.Length;
            var instances = new InstanceInfo[instanceCount];

	        for (int i = 0; i < instanceStrings.Length; i++)
	        {
		        var data = instanceStrings[i];
		        if (data.Length == 0)
			        continue;

		        var pair = data.Split(',');
		        var resourceNum = Convert.ToInt32(pair[0]);
		        var instanceName = pair[1];
		        instances[i] = new InstanceInfo
		        {
			        resourceNum = resourceNum,
			        name = instanceName,
		        };
	        }
	        return instances;
        }

        private static FrameData[] ReadFrames(XContainer xml, int instanceCount)
        {
            var elements = xml.Element("frames").Elements().ToArray();
            var framesCount = elements.Length;
            var frames = new FrameData[framesCount];

            for (int i = 0; i < framesCount; i++)
            {
                var prevFrame = (i > 0) ? frames[i - 1] : null;
                frames[i] = ReadFrame(elements[i], prevFrame, instanceCount);
            }

            return frames;
        }

        private static FrameData ReadFrame(XElement element, FrameData prevFrame, int totalInstanceCount)
        {
            var frame = new FrameData();
            frame.existingInstancesBits = new BitArray(totalInstanceCount, false);
            var labelsAttr = element.Attribute("labels");
			frame.labels = labelsAttr != null ? labelsAttr.Value.Split(',') : new string[] { };

            var frameData = element.Value;

            if (frameData.Length == 0)
            {
                frame.instances = new InstanceData[] { };
                return frame;
            }

            var instanceStrings = element.Value.Split('|');
            var instanceCount = instanceStrings.Length;
            frame.instances = new InstanceData[instanceCount];

			var prevFrameInstances = prevFrame != null
                                         ? prevFrame.instances.ToDictionary(it => it.id, it => it)
                                         : new Dictionary<int, InstanceData>();

            for (int i = 0; i < instanceCount; i++)
            {
                var instance = ReadInstance(instanceStrings[i], prevFrameInstances);
                frame.instances[i] = instance;
                frame.existingInstancesBits[instance.id] = true;
            }

            return frame;
        }

        private static InstanceData ReadInstance(string data, IDictionary<int, InstanceData> prevFrameInstances)
        {
            var instance = new InstanceData();
            var properties = data.Split(',');
            int propIndex = 0;

            instance.id = Convert.ToInt32(properties[propIndex]);

            InstanceData prevInstance;
            prevFrameInstances.TryGetValue(instance.id, out prevInstance);
            if (prevInstance == null)
                prevInstance = instance;

			instance.position.x = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.position.x);
			instance.position.y = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.position.y);
			instance.rotation = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.rotation);
			instance.scale.x = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.scale.x);
			instance.scale.y = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.scale.y);

			instance.color.rMult = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.rMult);
			instance.color.gMult = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.gMult);
			instance.color.bMult = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.bMult);
			instance.color.aMult = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.aMult);

			instance.color.rOffset = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.rOffset);
			instance.color.gOffset = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.gOffset);
			instance.color.bOffset = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.bOffset);
			instance.color.aOffset = ReadProperty(properties, ++propIndex, GetFloat, prevInstance.color.aOffset);

            return instance;
        }

		private static T ReadProperty<T>(string[] properties, int index, Func<string, T> func, T defaultValue)
        {
            if (index >= properties.Length)
                return defaultValue;

            var stringValue = properties[index];

            return stringValue.Length > 0
				? func(stringValue)
                : defaultValue;
        }

        private static float GetFloat(string value)
        {
            return Convert.ToSingle(value, CultureInfo.InvariantCulture);
        }
    }
}
