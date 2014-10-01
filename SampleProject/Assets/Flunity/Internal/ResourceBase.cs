using System;
using Flunity.Common;

namespace Flunity.Internal
{
	/// <summary>
	/// Base class for resources
	/// </summary>
	public abstract class ResourceBase : IResource
	{
		public string path { get; private set; }
		
		protected ResourceBase(string path)
		{
			this.path = path;
		}

		public ResourceBundle bundle { get; set; }

		public bool isLoaded { get; set; }
		
		public abstract void Load();

		public abstract void Unload();

		protected void EnsureLoaded()
		{
			if (!isLoaded)
				throw new Exception("Resource is not loaded: " + path);
		}

		public override string ToString()
		{
			return "Resource[" + path + "]";
		}
	}
}
