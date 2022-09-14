#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabFoldersToCategoryDropData
    {
        #region Private Variables
        private List<PrefabFolderDropData> _prefabFoldersDropDataCollection = new List<PrefabFolderDropData>();
        private PrefabCategory _destinationCategory;
        private bool _processSubfolders;
        #endregion

        #region Public Properties
        public List<PrefabFolderDropData> PrefabFoldersDropDataCollection { get { return new List<PrefabFolderDropData>(_prefabFoldersDropDataCollection); } }
        public PrefabCategory DestinationCategory { get { return _destinationCategory; } }
        #endregion

        #region Constructors
        public PrefabFoldersToCategoryDropData(PrefabCategory destinationCategory, bool processSubfolders)
        {
            _destinationCategory = destinationCategory;
            _processSubfolders = processSubfolders;
        }
        #endregion

        #region Public Methods
        public void FromLastDropOperation()
        {
            _prefabFoldersDropDataCollection.Clear();

            if (_processSubfolders) ExtractPrefabFolderDropDataFromDroppedFoldersAndChildFolders();
            else ExtractPrefabFolderDropDataFromFoldersOnly();
        }
        #endregion

        #region Private Methods
        private void ExtractPrefabFolderDropDataFromDroppedFoldersAndChildFolders()
        {
            HashSet<string> folderPaths = GetFolderAndChildFolderPaths(DragAndDrop.paths);
            foreach (string folderPath in folderPaths)
            {
                var prefabFolderDropData = new PrefabFolderDropData();
                prefabFolderDropData.FromFolderPath(folderPath, false);

                if (prefabFolderDropData.NumberOfValidPrefabs != 0) _prefabFoldersDropDataCollection.Add(prefabFolderDropData);
            }
        }

        private HashSet<string> GetFolderAndChildFolderPaths(string[] folderPaths)
        {
            var folderAndChildFolderPaths = new HashSet<string>();
            foreach(string folderPath in folderPaths)
            {
                List<string> paths = new List<string>(FileSystem.GetSubfolderPathsInFolder(folderPath, true));
                foreach(string path in paths)
                {
                    folderAndChildFolderPaths.Add(path);
                }
            }

            return folderAndChildFolderPaths;
        }

        private void ExtractPrefabFolderDropDataFromFoldersOnly()
        {
            string[] folderPaths = DragAndDrop.paths;
            foreach (string folderPath in folderPaths)
            {
                if(FileSystem.FolderExists(folderPath))
                {
                    var prefabFolderDropData = new PrefabFolderDropData();
                    prefabFolderDropData.FromFolderPath(folderPath, true);

                    if (prefabFolderDropData.NumberOfValidPrefabs != 0) _prefabFoldersDropDataCollection.Add(prefabFolderDropData);
                }
            }
        }
        #endregion
    }
}
#endif