using System;
using System.Collections.Generic;
using System.Reflection;
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
		private Dictionary<string, string[]> _descriptions;
		private Texture2D _texture;

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

			#if (UNITY_EDITOR || UNITY_STANDALONE) && !UNITY_WEBPLAYER
			if (FlashResources.isPlatformReloadingEnabled)
				AddBundleWatcher();
			#endif
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

			#if (UNITY_EDITOR || UNITY_STANDALONE) && !UNITY_WEBPLAYER
			if (_watcher != null)
				RemoveBundleWatcher();
			#endif
		}


		private void ReadAllDescriptions()
		{
			_descriptions = new Dictionary<string, string[]>();

			TryReadDescription(GetSheetFilePath());
			TryReadDescription(GetTimelineFilePath());
		}		

		private void TryReadDescription(string filePath)
		{
			if (FlashResources.logLevel <= LogLevel.DEBUG)
				Debug.Log("Reading description: " + filePath);

			var text = ResourceHelper.TryReadText(filePath);
			if (text == null)
			{
				if (FlashResources.logLevel <= LogLevel.DEBUG)
					Debug.Log("Description not found");
				return;
			}

			var elements = text.Split("\n---\n");
			foreach (var element in elements)
			{
				var lines = element.Split('\n');
				var resourcePath = lines[0];
				_descriptions[resourcePath] = lines;
			}
		}

		private void TryReadTexture()
		{
			if (FlashResources.isPlatformReloadingEnabled)
			{
				// File.ReadAllBytes is not available on WindowsPhone
				#if (UNITY_EDITOR || UNITY_STANDALONE) && !UNITY_WEBPLAYER
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
				#endif
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
			}
		}

		internal string[] GetDescription(string path)
		{
			if (_descriptions == null)
				throw new Exception("Descriptions for bundle is not loaded: " + name);

			string[] description;
			_descriptions.TryGetValue(path, out description);
			return description;
		}

		private string GetSheetFilePath()
		{
			var lowPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture");
			var highPath = PathUtil.Combine(FlashResources.bundlesRoot, name, "texture$hd");

			return FlashResources.useHighDefTextures && ResourceHelper.AssetExists(highPath + ".txt")
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

		internal Texture2D texture
		{
			get { return _texture; }
		}

		#region reloading
		#if (UNITY_EDITOR || UNITY_STANDALONE) && !UNITY_WEBPLAYER
		private FileSystemWatcher _watcher;
		
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

		#endif
		#endregion
	}
}