using System;
using System.Collections.Generic;
using Flunity.Common;
using Flunity.Utils;
using Flunity.Internal;
using UnityEngine;

namespace Flunity
{
	/// <summary>
	/// Contains set of resources which are loaded/unloaded simultaneously.
	/// </summary>
	public class ResourceBundle
	{
		/// <summary>
		/// Fired when bundle is loaded
		/// </summary>
		public event Action<ResourceBundle> Loaded;

		/// <summary>
		/// Fired when bundle is unloaded
		/// </summary>
		public event Action<ResourceBundle> Unloaded;

		public string name { get; protected set; }
				public Dictionary<string, IResource> resources { get; private set; }

		bool _isLoaded;

		public bool isLoaded
		{
			get { return _isLoaded; }
			private set
			{
				if (_isLoaded != value)
				{
					_isLoaded = value;

					if (_isLoaded)
						Loaded.Dispatch(this);
					else
						Unloaded.Dispatch(this);
				}
			}
		}

		public ResourceBundle(string name)
		{
			this.name = name;
			resources = new Dictionary<string, IResource>();
			isLoaded = false;
		}
		
		/// <summary>
		/// Calls FlashResources.LoadBundle(this);
		/// </summary>
		public void Load()
		{
			FlashResources.LoadBundle(this);
		}

		/// <summary>
		/// Calls FlashResources.UnloadBundle(this);
		/// </summary>
		public void Unload()
		{
			FlashResources.UnloadBundle(this);
		}

		internal void InternalLoad()
		{
			if (isLoaded)
				throw new Exception("Bundle is already loaded");

			LoadResources();

			isLoaded = true;
		}

		internal void InternalUnload()
		{
			if (!isLoaded)
				throw new Exception("Bundle is not loaded");

			UnloadResources();

			isLoaded = false;
		}

		protected virtual void LoadResources()
		{
			foreach (var resource in resources.Values)
			{
				LoadResource(resource);
			}

			if (FlashResources.logLevel <= LogLevel.DEBUG)
				Debug.Log(string.Format("Loaded {0} resources", resources.Count));
		}

		protected virtual void UnloadResources()
		{
			foreach (var resource in resources.Values)
			{
				UnloadResource(resource);
			}
		}

		protected void LoadResource(IResource resource)
		{
			if (resource.isLoaded)
				throw new Exception("Resource is already loaded: " + resource.path);

			resource.Load();
			resource.isLoaded = true;
		}

		private void UnloadResource(IResource resource)
		{
			if (!resource.isLoaded)
				throw new Exception("Resource is not loaded: " + resource.path);

			resource.Unload();
			resource.isLoaded = false;
		}

		/// <summary>
		/// Register resource in this bundle.
		/// </summary>
		public ResourceBundle AddResource(IResource resource)
		{
			if (resources.ContainsKey(resource.path))
				throw new Exception("Bundle already has such path: " + resource.path);

			resources[resource.path] = resource;

			resource.bundle = this;
			
			return this;
		}

		/// <summary>
		/// Register resources in this bundle.
		/// </summary>
		public ResourceBundle AddResources(IEnumerable<IResource> resources)
		{
			foreach (var resource in resources)
			{
				AddResource(resource);
			}
			return this;
		}

		internal T LoadContent<T>(string path) where T: UnityEngine.Object
		{
			if (FlashResources.logLevel <= LogLevel.DEBUG)
				Debug.Log("Loading: " + path);

			var content = UnityEngine.Resources.Load<T>(path);

			if (content == null)
			{
				if (FlashResources.logLevel <= LogLevel.DEBUG)
					Debug.Log("Not found: " + path);

			}

			return content;
		}

		internal virtual IResource GetResource(string resourcePath)
		{
			IResource resource;
			resources.TryGetValue(resourcePath, out resource);
			return resource;
		}

		public override string ToString()
		{
			return "ResourceBundle:" + name;
		}
	}
}
