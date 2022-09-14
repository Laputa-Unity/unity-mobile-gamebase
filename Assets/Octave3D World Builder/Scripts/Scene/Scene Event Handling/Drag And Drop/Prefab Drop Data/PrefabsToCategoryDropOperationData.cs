#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public class PrefabsToCategoryDropOperationData
    {
        #region Private Variables
        private List<Prefab> _validDroppedPrefabs = new List<Prefab>();
        private PrefabCategory _destinationCategory;
        #endregion

        #region Public Properties
        public List<Prefab> ValidDroppedPrefabs { get { return new List<Prefab>(_validDroppedPrefabs); } }
        public PrefabCategory DestinationCategory { get { return _destinationCategory; } }
        #endregion

        #region Constructors
        public PrefabsToCategoryDropOperationData(PrefabCategory destinationCategory)
        {
            _destinationCategory = destinationCategory;
        }
        #endregion

        #region Public Methods
        public void FromLastDropOperation()
        {
            ExtractAllValidDroppedPrefabs();
        }
        #endregion

        #region Private Methods
        private void ExtractAllValidDroppedPrefabs()
        {
            List<GameObject> validUnityPrefabs = PrefabValidator.GetValidPrefabsFromEntityCollection(DragAndDrop.objectReferences, false);
            _validDroppedPrefabs = PrefabFactory.Create(validUnityPrefabs);
        }
        #endregion
    }
}
#endif