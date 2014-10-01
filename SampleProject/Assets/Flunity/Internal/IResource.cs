using System;

namespace Flunity.Internal
{
	/// <summary>
	/// Base interface for all resources
	/// </summary>
	public interface IResource
	{
		/// <summary>
		/// Identifies resource
		/// </summary>
		String path { get; }

		/// <summary>
		/// Returns true if resiurce is loaded
		/// </summary>
		bool isLoaded { get; set; }

		/// <summary>
		/// Loads resource. Colled by ResourceBundle.
		/// Do not call directly.
		/// </summary>
		void Load();

		/// <summary>
		/// Disposes resource. Colled by ResourceBundle.
		/// Do not call directly.
		/// </summary>
		void Unload();

		/// <summary>
		/// Bundle this resource belongs to.
		/// Set by ResourceBundle.
		/// </summary>
		ResourceBundle bundle { get; set; }
	}
}