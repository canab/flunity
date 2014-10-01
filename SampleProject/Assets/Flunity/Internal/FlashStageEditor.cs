#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using Flunity.Utils;

namespace Flunity.Internal
{
	[CustomEditor(typeof(FlashStage))]
	public class FlashStageEditor : Editor
	{
		private static string[] _resources;
		private static DateTime _timeStamp = DateTime.MinValue;

		public override void OnInspectorGUI()
		{
			var stage = (FlashStage) target;
			stage.width = EditorGUILayout.IntField("Width", stage.width);
			stage.height = EditorGUILayout.IntField("Height", stage.height);

			if (_resources == null || _timeStamp != AssetsListener.timeStamp)
			{
				_resources = ReadResources();
				_timeStamp = AssetsListener.timeStamp;
			}

			stage.shader = (Shader) EditorGUILayout.ObjectField("Shader", stage.shader, typeof(Shader), false);

			var resNum = EditorGUILayout.Popup("Preview", GetResIndex(stage.resourcePath), _resources);
			stage.resourcePath = GetResName(resNum);

			EditorGUI.BeginDisabledGroup(stage.resourcePath.IsNullOrEmpty());
			stage.isPlaying = EditorGUILayout.Toggle("isPlaying", stage.isPlaying);
			EditorGUI.EndDisabledGroup();

			if (GUI.changed)
				EditorUtility.SetDirty(target);
		}

		string[] ReadResources()
		{
			var list = new List<string>(100) {"-"};
			var bundles = ReadBundles();
			foreach (var bundleName in bundles)
			{
				var bundleClass = FlashResources.GetBundleClass(bundleName);
				if (bundleClass == null)
					continue;

				var staticFields = bundleClass.GetFields(BindingFlags.Public
					| BindingFlags.NonPublic
					| BindingFlags.Static);

				foreach (var fieldInfo in staticFields)
				{
					var resource = fieldInfo.GetValue(null) as IDisplayResource;
					if (resource != null)
						list.Add(resource.path);
				}
			}

			return list.ToArray();
		}

		string[] ReadBundles()
		{
			var assets = AssetDatabase.FindAssets("t:Script", new []{ "Assets/Resources/FlashBundles" });
			var bundles = new string[assets.Length];

			for (int i = 0; i < assets.Length; i++)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(assets[i]);
				var fileName = assetPath.Substring(assetPath.LastIndexOf('/') + 1);
				var className = fileName.Substring(0, fileName.Length - 3);
				bundles[i] = className;
			}

			return bundles;
		}

		int GetResIndex(string resName)
		{
			var index = Array.IndexOf(_resources, resName);
			return _resources.ContainsIndex(index) ? index : 0;
		}

		string GetResName(int index)
		{
			return index > 0 && index < _resources.Length
				? _resources[index]
				: "";
		}
	}
}
#endif