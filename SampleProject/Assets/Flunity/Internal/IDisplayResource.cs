using Flunity;

namespace Flunity.Internal
{
	/// <summary>
	/// Interface for resources that can instantiate DisplayObject
	/// </summary>
	public interface IDisplayResource : IResource
	{
		DisplayObject CreateInstance();
	}
}

