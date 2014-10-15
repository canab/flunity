using System;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using Flunity.Utils;
using Flunity;

namespace Flunity.Internal
{
	internal static class ResourceHelper
	{
		public static string rootPath = ".";

		public static String GetDocPath(string localPath)
		{
			localPath = NormalizeSeparator(localPath);
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), localPath);
		}

		public static String NormalizeSeparator(string path)
		{
			return path.Replace('/', Path.DirectorySeparatorChar)
				.Replace('\\', Path.DirectorySeparatorChar);
		}

		public static XDocument ReadXml(string fullPath)
		{
			var xml = TryReadXml(fullPath);

			if (xml == null)
				throw new Exception("Resource not found: " + fullPath);

			return xml;
		}

		public static XDocument TryReadXml(string fullPath)
		{
			if (FlashResources.isReloadingEnabled)
			{
				if (!fullPath.EndsWith(".xml", StringComparison.Ordinal))
					fullPath = fullPath + ".xml";

				var globalPath = Path.GetFullPath(PathUtil.Combine("Assets", "Resources", fullPath));

				return File.Exists(globalPath)
					? XDocument.Load(globalPath)
					: null;
			}
			else
			{
				if (fullPath.EndsWith(".xml", StringComparison.Ordinal))
					fullPath = fullPath.Substring(0, fullPath.Length - 4);
				var asset = UnityEngine.Resources.Load<TextAsset>(fullPath);
				return asset != null
					? XDocument.Load(new StringReader(asset.text))
					: null;
			}
		}

		public static byte[] ReadBytes(string fullPath)
		{
			using (var fileStream = GetReadStream(fullPath))
			{
				using (var memoryStream = new MemoryStream())
				{
					fileStream.CopyTo(memoryStream);
					return memoryStream.ToArray();
				}
			}
		}

		public static FileStream GetWriteStream(string fullPath)
		{
			return File.Open(fullPath, FileMode.Create);
		}
		
		public static Stream GetReadStream(string fullPath)
		{
			#if ANDROID
			return Game.Activity.Assets.Open(fullPath);
			#else
			return File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
			#endif
		}

		public static bool AssetExists(string fullPath)
		{
			return File.Exists("Assets/Resources/" + fullPath);
		}
	}
}