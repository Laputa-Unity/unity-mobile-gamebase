#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToPathTileConectionDropEventHandler : DragAndDropEventHandler
    {
        #region Private Variables
        [SerializeField]
        private PrefabsToPathTileConectionButtonDropSettings _dropSettings;
        private ObjectPlacementPathTileConnectionType _destinationTileConnectionType;
        #endregion

        #region Public Properties
        public ObjectPlacementPathTileConnectionType DestinationTileConnectionType { get { return _destinationTileConnectionType; } set { _destinationTileConnectionType = value; } }
        public PrefabsToPathTileConectionButtonDropSettings DropSettings
        {
            get
            {
                if (_dropSettings == null) _dropSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<PrefabsToPathTileConectionButtonDropSettings>();
                return _dropSettings;
            }
        }
        #endregion

        #region Public Static Functions
        public static PrefabsToPathTileConectionDropEventHandler Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabsToPathTileConectionDropEventHandler;
        }
        #endregion

        #region Protected Methods
        protected override void PerformDrop()
        {
            if (ObjectPlacementPathTileConnectionSettingsChangeValidation.Validate(true))
            {
                List<GameObject> validUnityPrefabsInvolvedInDropOperation = PrefabValidator.GetValidPrefabsFromEntityCollection(DragAndDrop.objectReferences, false);
                if (validUnityPrefabsInvolvedInDropOperation.Count != 0) PerformDropUsingFirstPrefabInValidUnityPrefabCollection(validUnityPrefabsInvolvedInDropOperation);
            }
        }
        #endregion

        #region Private Methods
        private void PerformDropUsingFirstPrefabInValidUnityPrefabCollection( List<GameObject> validUnityPrefabs)
        {
            GameObject firstValidUnityPrefab = GetFirstUnityPrefabFromValidUnityPrefabsCollection(validUnityPrefabs);
            PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(firstValidUnityPrefab);

            if (categoryWhichContainsPrefab != null) AssignPrefabToDestinationTileConnection(categoryWhichContainsPrefab.GetPrefabByUnityPrefab(firstValidUnityPrefab));
            else
            {
                Prefab firstValidPrefab = GetFirstPrefabFromValidPrefabsCollection(PrefabFactory.Create(validUnityPrefabs));
                CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationTileConnection(firstValidPrefab);
            }
        }

        private GameObject GetFirstUnityPrefabFromValidUnityPrefabsCollection(List<GameObject> validUnityPrefabs)
        {
            return validUnityPrefabs[0];
        }

        private Prefab GetFirstPrefabFromValidPrefabsCollection(List<Prefab> validPrefabs)
        {
            return validPrefabs[0];
        }

        private void CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationTileConnection(Prefab prefab)
        {
            PrefabWithPrefabCategoryAssociationQueue.Instance.Enqueue(PrefabWithPrefabCategoryAssociationFactory.Create(prefab, DropSettings.DestinationCategoryForDroppedPrefabs));
            AssignPrefabToDestinationTileConnection(prefab);
        }

        private void AssignPrefabToDestinationTileConnection(Prefab prefab)
        {
            ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings = PathObjectPlacement.Get().PathSettings.TileConnectionSettings.GetSettingsForTileConnectionType(_destinationTileConnectionType);
            UndoEx.RecordForToolAction(tileConnectionTypeSettings);
            tileConnectionTypeSettings.Prefab = prefab;
        }
        #endregion
    }
}
#endif