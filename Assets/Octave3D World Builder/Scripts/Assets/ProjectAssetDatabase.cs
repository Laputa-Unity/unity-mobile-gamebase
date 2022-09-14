#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ProjectAssetDatabase
    {
        #region Public Static Functions
        public static List<GameObject> LoadAllValidPrefabsInFolder(string folderName, bool includeSubfolders = true, bool showProgressBar = false)
        {
            if(includeSubfolders)
            {
                string progressTitle = "Loading assets";
                string progressMessage = "Please wait...";
                if (showProgressBar) EditorUtility.DisplayProgressBar(progressTitle, progressMessage, 0.0f);
                List<UnityEngine.Object> assets = ProjectAssetDatabase.LoadAssetsInFolder(folderName, "GameObject", "");
                if (showProgressBar) EditorUtility.DisplayProgressBar(progressTitle, progressMessage, 0.5f);
                List<GameObject> unityPrefabs = GetValidPrefabsWhichExistAmongstAssets(assets);
                if (showProgressBar) EditorUtility.ClearProgressBar();
                if (unityPrefabs.Count == 0) return new List<GameObject>();

                return unityPrefabs;
            }
            else
            {
                List<string> assetPaths = FileSystem.GetAllFilesInFolder(folderName);
                List<GameObject> validPrefabs = GetValidPrefabsWhichExistAmongstAssets(assetPaths, showProgressBar);

                return validPrefabs;
            }
        }

        public static bool IsAssetPrefab(string assetFilePath)
        {
            string prefabExtension = ".prefab";
            if (assetFilePath.Length <= prefabExtension.Length) return false;
            else return assetFilePath.EndsWith(prefabExtension);
        }

        public static GameObject LoadPrefabWithNameInFolder(string prefabName, string folderName, bool includeSubfolders = true)
        {
            List<GameObject> prefabsInFolder = LoadAllValidPrefabsInFolder(folderName, includeSubfolders);
            return prefabsInFolder.Find(item => item.name == prefabName);
        }

        public static GameObject CreatePrefab(GameObject hierarchyRoot, string prefabName, string destinationFolderName)
        {
            string fullPrefabName = destinationFolderName + "/" + prefabName + ".prefab";

            UnityEngine.Object prefab = PrefabUtility.SaveAsPrefabAsset(hierarchyRoot, fullPrefabName);
            AssetDatabase.Refresh();

            return prefab as GameObject;
        }

        public static Texture2D LoadTextureAtPath(string texturePath)
        {
            string folderName = Path.GetDirectoryName(texturePath);
            string textureName = Path.GetFileName(texturePath);

            int dotIndex = textureName.IndexOf('.');
            if (dotIndex >= 0) textureName = textureName.Remove(dotIndex);

            List<UnityEngine.Object> assets = LoadAssetsInFolder(folderName, "Texture2D", textureName);
            if (assets.Count == 0) return null;

            return assets[0] as Texture2D;
        }
        #endregion

        #region Private Static Functions
        /// <summary>
        /// Returns the assets which reside in the folder with the specified name and which meet a certain criteria.
        /// </summary>
        /// <param name="assetFolderName">
        /// The folder from which the assests will be retrieved.
        /// </param>
        /// <param name="assetTypeFilter">
        /// A string representing the type of assets to load. If null, assets of all types will be loaded.
        /// Please see http://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html for more detailed info.
        /// </param>
        /// <param name="assetNameFilter">
        /// Only assets that have a name which contains 'assetNameFilter' in it will be returned.
        /// Please see http://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html for more detailed info.
        /// </param>
        /// <returns>
        /// The list of assets in the specified folder. The list will be empty if no assets reside in the specified 
        /// folder or if the assets which reside there don't meet the specified criteria.
        /// </returns>
        private static List<UnityEngine.Object> LoadAssetsInFolder(string assetFolderName, string assetTypeFilter, string assetNameFilter)
        {
            string[] assetGUIDs = AssetDatabase.FindAssets(assetNameFilter + " t:" + assetTypeFilter, new string[] { assetFolderName });
            return ConvertGUIDsToAssets(assetGUIDs);
        }

        private static List<UnityEngine.Object> ConvertGUIDsToAssets(string[] assetGUIDs)
        {
            var assets = new List<UnityEngine.Object>();
            foreach (var guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object));

                if (asset != null) assets.Add(asset);
            }

            return assets;
        }

        private static List<GameObject> GetValidPrefabsWhichExistAmongstAssets(List<UnityEngine.Object> assets)
        {
            var validPrefabs = new List<GameObject>();
            foreach (var asset in assets)
            {
                GameObject prefabFromAsset = asset as GameObject;
                if (PrefabValidator.ValidatePrefab(prefabFromAsset, false)) validPrefabs.Add(prefabFromAsset);
            }

            return validPrefabs;
        }

        private static List<GameObject> GetValidPrefabsWhichExistAmongstAssets(List<string> assetPaths, bool showProgressBar = false)
        {
            var validPrefabs = new List<GameObject>();
            if(showProgressBar)
            {
                for(int pathIndex = 0; pathIndex < assetPaths.Count; ++pathIndex)
                {
                    string assetPath = assetPaths[pathIndex];
                    EditorUtility.DisplayProgressBar("Loading assets", "Please wait... (" + assetPath + ")", (pathIndex + 1) / (float)assetPaths.Count);
                    if (IsAssetPrefab(assetPath))
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        if (PrefabValidator.ValidatePrefab(prefab, false)) validPrefabs.Add(prefab);
                    }
                }
            }
            else
            {
                foreach (string assetPath in assetPaths)
                {
                    if (IsAssetPrefab(assetPath))
                    {
                        GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                        if (PrefabValidator.ValidatePrefab(prefab, false)) validPrefabs.Add(prefab);
                    }
                }
            }

            return validPrefabs;
        }
        #endregion
    }
}
#endif