#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabsToCategoryDropOperation : IPrefabsDropOperation
    {
        #region Private Variables
        private PrefabsToCategoryDropOperationData _dropData;
        private List<Prefab> _validDroppedPrefabs;
        #endregion

        #region Constructors
        public PrefabsToCategoryDropOperation(PrefabsToCategoryDropOperationData dropData)
        {
            _dropData = dropData;
            _validDroppedPrefabs = _dropData.ValidDroppedPrefabs;
        }
        #endregion

        #region Public Methods
        public void Perform()
        {
            DropPrefabsInDestinationCategory();
        }
        #endregion

        #region Private Methods
        private void DropPrefabsInDestinationCategory()
        {
            UndoEx.RecordForToolAction(_dropData.DestinationCategory);
            _dropData.DestinationCategory.AddPrefabCollection(_validDroppedPrefabs);
            AssociateDroppedPrefabsWithTags();
        }

        private void AssociateDroppedPrefabsWithTags()
        {
            PrefabsToCategoryDropSettings prefabsDropSettings = PrefabsToCategoryDropEventHandler.Get().PrefabsDropSettings;
            PrefabActions.AssociatePrefabsWithTagCollection(_validDroppedPrefabs, prefabsDropSettings.TagNamesForDroppedPrefabs);
        }
        #endregion
    }
}
#endif