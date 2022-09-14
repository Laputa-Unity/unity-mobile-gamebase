#if UNITY_EDITOR
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabFolderDropData
    {
        #region Private Variables
        private string _folderPath;
        private string _folderNameInPath;
        private List<Prefab> _validPrefabs = new List<Prefab>();
        private bool _extractPrefabsInSubfolders;
        #endregion

        #region Public Properties
        public string FolderPath { get { return _folderPath; } }
        public string FolderNameInPath { get { return _folderNameInPath; } }
        public int NumberOfValidPrefabs { get { return _validPrefabs.Count; } }
        public List<Prefab> ValidPrefabs { get { return new List<Prefab>(_validPrefabs); } }
        #endregion

        #region Public Methods
        public void FromFolderPath(string folderPath, bool extractPrefabsInSubfolders)
        {
            _folderPath = folderPath;
            _folderNameInPath = FileSystem.GetLastFolderNameInPath(_folderPath);
            _extractPrefabsInSubfolders = extractPrefabsInSubfolders;

            ExtractAllValidPrefabsInDroppedFolder();
        }
        #endregion

        #region Private Methods
        private void ExtractAllValidPrefabsInDroppedFolder()
        {
            List<GameObject> validUnityPrefabs = ProjectAssetDatabase.LoadAllValidPrefabsInFolder(FolderPath, _extractPrefabsInSubfolders, true);
            _validPrefabs = PrefabFactory.Create(validUnityPrefabs);
        }
        #endregion
    }
}
#endif