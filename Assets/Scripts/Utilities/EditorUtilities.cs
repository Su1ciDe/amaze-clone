using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Utilities
{
	public class EditorUtilities : MonoBehaviour
	{
#if UNITY_EDITOR
		/// <summary>
		/// Returns all the assets object of the type at the given path.
		/// </summary>
		/// <param name="path">Folder's path</param>
		public static IEnumerable<T> LoadAllAssetsFromPath<T>(string path) where T : Object
		{
			var filePaths = System.IO.Directory.GetFiles(path);

			foreach (var filePath in filePaths)
			{
				var obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(T));
				if (obj is T asset)
					yield return asset;
			}
		}
#endif
	}
}