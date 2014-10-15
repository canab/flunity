using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Flunity.Common;
using Flunity.Utils;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Manages loading/unloading of resource bundles
	/// </summary>
	public class FlashResources
	{
		/// <summary>
		/// Fired when resources have been modified and reloaded at runtime.
		/// </summary>
		public static event Action Reloaded;

		/// <summary>
		/// Change for more verbose logging, see LogLevel.
		/// </summary>
		public static LogLevel logLevel = LogLevel.INFO;

		/// <summary>
		/// Determines whether hi-def textures will be used.
		/// Should be set depending on device's resolution.
		/// </summary>
		public static bool useHighDefTextures = false;

		/// <summary>
		/// Path where flash bundles are located.
		/// Relative to Resources folder.
		/// </summary>
		public static string bundlesRoot = "FlashBundles";

		internal static readonly ActionsInvoker reloadingInvoker = new ActionsInvoker();
		internal static bool isReloadingEnabled = false;
		internal static bool isReloadingPerformed = false;

		private static readonly List<ResourceBundle> _currentBundles = new List<ResourceBundle>();
		private static readonly ReadOnlyCollection<ResourceBundle> _currentBundlesView
			= new ReadOnlyCollection<ResourceBundle>(_currentBundles);

		internal static bool isPlatformReloadingEnabled
		{
			get
			{
				#if UNITY_EDITOR || UNITY_STANDALONE
				return isReloadingEnabled;
				#else
				return false;
				#endif
			}
		}

		internal static void reloadPendingResources()
		{
			if (reloadingInvoker.hasActions)
			{
				reloadingInvoker.InvokeActions();
				isReloadingPerformed = true;
				Reloaded.Dispatch();
			}
		}

		internal static Type GetBundleClass(string bundleName)
		{
			return Type.GetType("FlashBundles." + bundleName);
		}

		internal static ResourceBundle GetBundleInstance(string bundleName)
		{
			var bundleClass = GetBundleClass(bundleName);
			if (bundleClass == null)
				return null;

			var instanceProp = bundleClass.GetField("instance", BindingFlags.Public | BindingFlags.Static);
			if (instanceProp == null)
				return null;

			return (ResourceBundle) instanceProp.GetValue(null);
		}

		/// <summary>
		/// Searches for resource in loaded bundles. Returns null if not found.
		/// </summary>
		public static IResource GetResource(string resourcePath)
		{
			return GetResource<IResource>(resourcePath);
		}

		internal static string GetFullPath(string path)
		{
			return path;
		}

		/// <summary>
		/// Searches for resource in loaded bundles. Returns null if not found.
		/// </summary>
		public static T GetResource<T>(string resourcePath) where T : class, IResource
		{
			foreach (var bundle in _currentBundles)
			{
				var resource = bundle.GetResource(resourcePath);
				if (resource != null)
					return (T)resource;
			}

			return null;
		}

		/// <summary>
		/// Loads bundles which are specified and not loaded yet.
		/// Unloads bundles which are loaded and not specified.
		/// </summary>
		public static void UseBundles(params ResourceBundle[] bundlesToUse)
		{
			var bundlesToRemove = _currentBundles
				.Where(it => !bundlesToUse.Contains(it))
				.ToList();

			var bundlesToAdd = bundlesToUse
				.Where(it => !_currentBundles.Contains(it))
				.ToList();

			bundlesToRemove.ForEach(InternalUnload);

			GC.Collect();
			bundlesToAdd.ForEach(InternalLoad);
			LogCurrentBundles();
		}

		/// <summary>
		/// Loads bundle if it is not loaded.
		/// </summary>
		public static void LoadBundle(ResourceBundle bundle)
		{
			if (!bundle.isLoaded)
				InternalLoad(bundle);
		}

		private static void InternalLoad(ResourceBundle bundle)
		{
			if (FlashResources.logLevel <= LogLevel.INFO)
				Debug.Log("Loading: " + bundle.name);

			_currentBundles.Add(bundle);

			bundle.InternalLoad();
		}

		/// <summary>
		/// Unloads bundle if it is loaded.
		/// </summary>
		public static void UnloadBundle(ResourceBundle bundle)
		{
			if (bundle.isLoaded)
				InternalUnload(bundle);
		}

		private static void InternalUnload(ResourceBundle bundle)
		{
			if (FlashResources.logLevel <= LogLevel.INFO)
				Debug.Log("Unloading: " + bundle.name);

			_currentBundles.Remove(bundle);

			bundle.InternalUnload();
		}

		/// <summary>
		/// Unloads all loaded bundles.
		/// </summary>
		public static void UnloadAllBundles()
		{
			while (_currentBundles.Count > 0)
			{
				InternalUnload(_currentBundles[0]);
			}
		}

		private static void LogCurrentBundles()
		{
			var usedNames = _currentBundles.Select(it => it.name).ToArray();

			if (FlashResources.logLevel <= LogLevel.INFO)
				Debug.Log("Used: " + string.Join(", ", usedNames));
		}

		/// <summary>
		/// Returns loaded bundles.
		/// </summary>
		public static ReadOnlyCollection<ResourceBundle> currentBundles
		{
			get { return _currentBundlesView; }
		}
	}
}