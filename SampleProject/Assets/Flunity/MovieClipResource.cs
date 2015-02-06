using System;
using Flunity;
using Flunity.Internal;

namespace Flunity
{
	/// <summary>
	/// Holds data exported from flash as MovieClip.
	/// </summary>
	public class MovieClipResource : ResourceBase, IDisplayResource
	{
		private TimeLine _timeLine;
		private Type _mappedClass;

		public MovieClipResource(string path)
			: base(path)
		{
			var className = path.Substring(path.LastIndexOf('/') + 1);
			var fullName = "FlashBundles." + className;
			_mappedClass = Type.GetType(fullName);
		}

		public override void Load()
		{
			if (_timeLine != null)
				return;
			
			var description = ReadDescription();
			if (description == null)
				throw new Exception("Resource not found: " + path);

			_timeLine = MovieClipParser.ReadTimeLine(description);
		}

		private string[] ReadDescription()
		{
			var contentBundle = bundle as ContentBundle;
			if (contentBundle != null)
				return contentBundle.GetDescription(path);
			
			return ResourceHelper.ReadText(path + ".txt").Split('\n');
		}

		public override void Unload()
		{
			_timeLine = null;
		}

		/// <summary>
		/// Creates MovieClip instance.
		/// </summary>
		public DisplayObject CreateInstance()
		{
			return _mappedClass != null
				? (MovieClip) Activator.CreateInstance(_mappedClass)
				: new MovieClip(this);
		}

		internal TimeLine timeLine
		{
			get
			{
				EnsureLoaded();
				return _timeLine;
			}
		}
		
		/// <summary>
		/// Number of frames in timeline
		/// </summary>
		public int totalFrames
		{
			get
			{
				EnsureLoaded();
				return _timeLine.frames.Length;
			}
		}
	}
}