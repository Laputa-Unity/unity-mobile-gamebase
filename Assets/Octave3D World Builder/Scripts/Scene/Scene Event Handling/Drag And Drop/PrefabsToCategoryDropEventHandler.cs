#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToCategoryDropEventHandler : DragAndDropEventHandler
    {
        #region Private Variables
        [SerializeField]
        private PrefabsToCategoryDropSettings _prefabsDropSettings;
        [SerializeField]
        private PrefabFoldersToCategoryDropSettings _prefabFoldersDropSettings;
        #endregion

        #region Public Properties
        public PrefabsToCategoryDropSettings PrefabsDropSettings
        {
            get
            {
                if (_prefabsDropSettings == null) _prefabsDropSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabsToCategoryDropSettings>();
                return _prefabsDropSettings;
            }
        }
        public PrefabFoldersToCategoryDropSettings PrefabFoldersDropSettings
        {
            get
            {
                if (_prefabFoldersDropSettings == null) _prefabFoldersDropSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabFoldersToCategoryDropSettings>();
                return _prefabFoldersDropSettings;
            }
        }
        #endregion

        #region Public Static Functions
        public static PrefabsToCategoryDropEventHandler Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabsToCategoryDropEventHandler;
        }
        #endregion

        #region Protected Methods
        protected override void PerformDrop()
        {
            PrefabCategory activePrefabCategory = PrefabCategoryDatabase.Get().ActivePrefabCategory;
            PerformPrefabsToCategoryDropOperation(activePrefabCategory);
            PerformPrefabFoldersToCategoryDropOperation(activePrefabCategory);
            PrefabTagsWindow.Get().RepaintOctave3DWindow();
        }
        #endregion

        #region Private Methods
        private void PerformPrefabsToCategoryDropOperation(PrefabCategory prefabCategory)
        {
            var prefabsDropData = new PrefabsToCategoryDropOperationData(prefabCategory);
            prefabsDropData.FromLastDropOperation();

            if(prefabsDropData.ValidDroppedPrefabs.Count != 0)
            {
                var prefabsDropOperation = new PrefabsToCategoryDropOperation(prefabsDropData);
                prefabsDropOperation.Perform();

                PrefabPreviewTextureCache.Get().GeneratePreviewsForPrefabCollection(prefabsDropData.ValidDroppedPrefabs, true);
            }
        }

        private void PerformPrefabFoldersToCategoryDropOperation(PrefabCategory prefabCategory)
        {
            var prefabFoldersDropData = new PrefabFoldersToCategoryDropData(prefabCategory, PrefabFoldersDropSettings.ProcessSubfolders);
            prefabFoldersDropData.FromLastDropOperation();

            var dropDataCollection = prefabFoldersDropData.PrefabFoldersDropDataCollection;
            if (dropDataCollection.Count != 0)
            {
                var prefabFoldersDropOperation = new PrefabFoldersToCategoryDropOperation(prefabFoldersDropData);
                prefabFoldersDropOperation.Perform();

                foreach(var dropData in dropDataCollection)
                {
                    PrefabPreviewTextureCache.Get().GeneratePreviewsForPrefabCollection(dropData.ValidPrefabs, true);
                }
            }
        }
        #endregion
    }
}
#endif
