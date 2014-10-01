using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using UnityEngine;
using System.IO;
using Flunity.Common;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Bundle that contains resources as static declarations.
	/// Typically is generated from .swf files.
	/// </summary>
	public class ContentBundle : ResourceBundle
	{
		private Dictionary<string, XElement> _descriptions;
		private Texture2D _texture;
		private FileSystemWatcher _watcher;
		
		public ContentBundle() : base("")
		{
			name = GetType().Name;
			AddStaticResources();
		}

		private void AddStaticResources()
		{
			var members = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
			foreach (var fieldInfo in members)
			{
				var resource = fieldInfo.GetValue(null) as IResource;
				if (resource != null)
					AddResource(resource);
			}
		}

		protected override void LoadResources()
		{
			if (_descriptions == null)
				ReadAllDescriptions();

			TryReadTexture();

			if (_descriptions.Count == 0)
				Debug.LogWarning("Bundle is empty!");

			base.LoadResources();

			if (FlashResources.isReloadingEnabled)
				AddBundleWatcher();
		}

		protected override void UnloadResources()
		{
			base.UnloadResources();

			if (_texture != null)
			{
				UnityEngine.Object.DestroyImmediate(_texture);
				//UnityEngine.Resources.UnloadAsset(_texture);
				_texture = null;
			}

			if (_watcher != null)
				RemoveBundleWatcher();
		}


		private void ReadAllDescriptions()
		{
			_descriptions = new Dictionary<string, XElement>();

			TryReadDescription(GetSheetFilePath());
			TryReadDescription(GetTimelineFilePath());
		}		

		private void TryReadDescription(string fileName)
		{
			var fullPath = FlashResources.GetFullPath(fileName);

			if (FlashResources.logLevel <= LogLevel.DEBUG)
				Debug.Log("Reading description: " + fullPath);

			var xml = ResourceHelper.TryReadXml(fullPath);
			if (xml == null)
			{
				if (FlashResources.logLevel <= LogLevel.DEBUG)
					Debug.Log("Description not found");
				return;
			}

			foreach (var node in xml.Root.Elements())
			{
				var resourcePath = node.Attribute("path").Value;
				_descriptions[resourcePath] = node;
			}
		}

		private void TryReadTexture()
		{
			if (FlashResources.isReloadingEnabled)
			{
				var localPath = PathUtil.Combine("Assets", "Resources", GetTextureFilePath() + ".png.bytes");
				var filePath = Path.GetFullPath(localPath);

				if (File.Exists(filePath))
				{
					var bytes = File.ReadAllBytes(filePath);
					_texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					_texture.LoadImage(bytes);
				}
				else
				{
					_texture = null;
				}
			}
			else
			{
				var bytesAsset = LoadContent<TextAsset>(GetTextureFilePath() + ".png");
				if (bytesAsset != null)
				{
					_texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
					_texture.LoadImage(bytesAsset.bytes);
				}
				UnityEngine.Resources.UnloadAsset(bytesAsset);

				//_texture = LoadContent<Texture2D>(GetTextureFilePath());
			}
		}

		internal XElement GetDescription(string path)
		{
			if (_descriptions == null)
				throw new Exception("Descriptions for bundle is not loaded: " + name);

			XElement description;
			_descriptions.TryGetValue(path, out description);
			return description;
		}

		internal TextureInfo GetTexture(string path)
		{
			if (_descriptions == null)
				throw new Exception("Descriptions for bundle is not loaded: " + name);

			if (_texture == null)
				throw new Exception("Texture for bundle is not loaded: " + name);

			XElement description;
			if (_descriptions.TryGetValue(path, out description))
			{
				var isHd = description.Attribute("hd").Value == "true";
				return new TextureInfo(_texture, scale: isHd ? 0.5f : 1);
			}

			throw new Exception("Texture path not found: " + path);
		}

		private string GetSheetFilePath()
		{
			var lowPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture");
			var highPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture$hd");

			return FlashResources.useHighDefTextures && ResourceHelper.AssetExists(highPath + ".xml")
				? highPath
				: lowPath;
		}

		private string GetTextureFilePath()
		{
			var lowPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture");
			var highPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture$hd");

			return FlashResources.useHighDefTextures && ResourceHelper.AssetExists(highPath + ".png.bytes")
				? highPath
				: lowPath;
		}

		private string GetTimelineFilePath()
		{
			return PathUtil.Combine(FlashResources.bundlesRoot, name, "timeline");
		}

		#region reloading

		void AddBundleWatcher()
		{
			var path = PathUtil.Combine("Assets", "Resources", FlashResources.bundlesRoot, name);

			_watcher = new FileSystemWatcher(Path.GetFullPath(path), ".bundle");
			_watcher.Changed += OnAssetChanged;
			_watcher.Created += OnAssetChanged;
			_watcher.Renamed += OnAssetChanged;
			_watcher.Deleted += OnAssetChanged;
			_watcher.EnableRaisingEvents = true;
		}

		void RemoveBundleWatcher()
		{
			_watcher.Changed -= OnAssetChanged;
			_watcher.Created -= OnAssetChanged;
			_watcher.Renamed -= OnAssetChanged;
			_watcher.Deleted -= OnAssetChanged;
			_watcher.EnableRaisingEvents = false;
			_watcher.Dispose();
			_watcher = null;
		}

		void OnAssetChanged(object sender, FileSystemEventArgs e)
		{
			RemoveBundleWatcher();
			FlashResources.reloadingInvoker.AddAction(Reload);
		}

		private void Reload()
		{
			if (FlashResources.logLevel <= LogLevel.DEBUG)
				Debug.LogWarning("Reloading...");

			_descriptions = null;
			FlashResources.UnloadBundle(this);
			FlashResources.LoadBundle(this);
			AddBundleWatcher();
		}

		#endregion
	}
}