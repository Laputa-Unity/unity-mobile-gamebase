#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabFoldersToCategoryDropOperation : IPrefabsDropOperation
    {
        #region Private Variables
        private PrefabFoldersToCategoryDropData _dropData;
        private PrefabCategory _lastCreatedCategory;
        private PrefabFoldersToCategoryDropSettings _prefabFoldersDropSettings;
        #endregion

        #region Constructors
        public PrefabFoldersToCategoryDropOperation(PrefabFoldersToCategoryDropData dropData)
        {
            _dropData = dropData;
            _prefabFoldersDropSettings = PrefabsToCategoryDropEventHandler.Get().PrefabFoldersDropSettings;
        }
        #endregion

        #region Public Methods
        public void Perform()
        {
            if (_dropData != null && _dropData.PrefabFoldersDropDataCollection.Count != 0)
            {
                if(_prefabFoldersDropSettings.CreatePrefabCategoriesFromDroppedFolders) UndoEx.RecordForToolAction(PrefabCategoryDatabase.Get());
                DropPrefabFolders();
                if (ShouldActivateLastPrefabCategoryAfterDrop()) PrefabCategoryDatabase.Get().SetActivePrefabCategory(_lastCreatedCategory);
            }
        }
        #endregion

        #region Private Methods
        private bool ShouldActivateLastPrefabCategoryAfterDrop()
        {
            return _lastCreatedCategory != null && _prefabFoldersDropSettings.ActivateLastCreatedCategory;
        }

        private void DropPrefabFolders()
        {
            List<PrefabFolderDropData> prefabFoldersDropDataCollection = _dropData.PrefabFoldersDropDataCollection;
            for (int folderDropDataIndex = 0; folderDropDataIndex < prefabFoldersDropDataCollection.Count; ++folderDropDataIndex)
            {
                EditorUtility.DisplayProgressBar("Processing dropped folders", "Please wait... (" + prefabFoldersDropDataCollection[folderDropDataIndex].FolderNameInPath + ")", (float)folderDropDataIndex / prefabFoldersDropDataCollection.Count);
                DropPrefabFolder(prefabFoldersDropDataCollection[folderDropDataIndex]);
            }

            EditorUtility.ClearProgressBar();
        }

        private void DropPrefabFolder(PrefabFolderDropData prefabFolderDropData)
        {
            DropPrefabFolderToCategory(prefabFolderDropData);

            if (!_prefabFoldersDropSettings.CreatePrefabTagsForEachDroppedFolder) PrefabActions.AssociatePrefabsWithTagCollection(prefabFolderDropData.ValidPrefabs, _prefabFoldersDropSettings.TagNamesForDroppedFolders);
            else CreateTagsForDroppedFolderIfNecessaryAndAssociate(prefabFolderDropData);
        }

        private void DropPrefabFolderToCategory(PrefabFolderDropData prefabFolderDropData)
        {
            if (_prefabFoldersDropSettings.CreatePrefabCategoriesFromDroppedFolders) CreateNewCategoryFromDroppedFolderAndAssignPrefabs(prefabFolderDropData);
            else
            {
                UndoEx.RecordForToolAction(_dropData.DestinationCategory);
                _dropData.DestinationCategory.AddPrefabCollection(prefabFolderDropData.ValidPrefabs);
            }
        }

        private void CreateNewCategoryFromDroppedFolderAndAssignPrefabs(PrefabFolderDropData prefabFolderDropData)
        {
            PrefabCategory newPrefabCategory = PrefabCategoryDatabase.Get().CreatePrefabCategory(prefabFolderDropData.FolderNameInPath);
            newPrefabCategory.SetPathFolderNames(FileSystem.GetFolderNamesInPath(prefabFolderDropData.FolderPath));
            newPrefabCategory.AddPrefabCollection(prefabFolderDropData.ValidPrefabs);
            _lastCreatedCategory = newPrefabCategory;
        }

        private void CreateTagsForDroppedFolderIfNecessaryAndAssociate(PrefabFolderDropData prefabFolderDropData)
        {
            string lastFolderNameInFoderPath = prefabFolderDropData.FolderNameInPath;
            if (PrefabTagDatabase.Get().ContainsPrefabTag(lastFolderNameInFoderPath))
                PrefabActions.AssociatePrefabsWithTag(prefabFolderDropData.ValidPrefabs, lastFolderNameInFoderPath);
            else
            {
                UndoEx.RecordForToolAction(PrefabTagDatabase.Get());
                PrefabTag prefabTag = PrefabTagDatabase.Get().CreatePrefabTag(lastFolderNameInFoderPath);
                PrefabActions.AssociatePrefabsWithTag(prefabFolderDropData.ValidPrefabs, prefabTag);
            }
        }
        #endregion
    }
}
#endif