#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class PrefabsToDecorPaintBrushEventHandler : DragAndDropEventHandler
    {
        public enum DropDestination
        {
            Brush = 0,
            Element
        }

        #region Private Variables
        private DecorPaintObjectPlacementBrushElement _destinationDecorPaintBrushElement;
        private DecorPaintObjectPlacementBrush _destinationBrush;

        private DropDestination _dropDestination = DropDestination.Brush;
        #endregion

        #region Public Properties
        public DecorPaintObjectPlacementBrushElement DestinationDecorPaintBrushElement { get { return _destinationDecorPaintBrushElement; } set { _destinationDecorPaintBrushElement = value; } }
        public DecorPaintObjectPlacementBrush DestinationBrush { get { return _destinationBrush; } set { _destinationBrush = value; } }
        public DropDestination DropDest { get { return _dropDestination; } set { _dropDestination = value; } }
        #endregion

        #region Public Static Functions
        public static PrefabsToDecorPaintBrushEventHandler Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabsToDecorPaintBrushEventHandler;
        }
        #endregion

        #region Protected Methods
        protected override void PerformDrop()
        {
            if(_dropDestination == DropDestination.Element)
            {
                if (_destinationDecorPaintBrushElement == null) return;

                List<GameObject> validUnityPrefabsInvolvedInDropOperation = PrefabValidator.GetValidPrefabsFromEntityCollection(DragAndDrop.objectReferences, false);
                if (validUnityPrefabsInvolvedInDropOperation.Count != 0) PerformDropUsingFirstPrefabInValidUnityPrefabCollection(validUnityPrefabsInvolvedInDropOperation);
            }
            else
            {
                if (_destinationBrush == null) return;

                List<GameObject> validUnityPrefabs = PrefabValidator.GetValidPrefabsFromEntityCollection(DragAndDrop.objectReferences, false);
                if (validUnityPrefabs.Count != 0)
                {
                    UndoEx.RecordForToolAction(_destinationBrush);
                    foreach(GameObject unityPrefab in validUnityPrefabs)
                    {
                        DecorPaintObjectPlacementBrushElement newElement = _destinationBrush.CreateNewElement();

                        PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(unityPrefab);
                        if (categoryWhichContainsPrefab != null) newElement.Prefab = categoryWhichContainsPrefab.GetPrefabByUnityPrefab(unityPrefab);
                        else
                        {
                            Prefab prefab = PrefabFactory.Create(unityPrefab);
                            UndoEx.RecordForToolAction(_destinationBrush.DestinationCategoryForElementPrefabs);
                            PrefabWithPrefabCategoryAssociationQueue.Instance.Enqueue(PrefabWithPrefabCategoryAssociationFactory.Create(prefab, _destinationBrush.DestinationCategoryForElementPrefabs));
                            newElement.Prefab = prefab;
                        }
                    }                 
                }
            }

            Octave3DWorldBuilder.ActiveInstance.RepaintAllEditorWindows();
        }
        #endregion

        #region Private Methods
        private void PerformDropUsingFirstPrefabInValidUnityPrefabCollection(List<GameObject> validUnityPrefabs)
        {
            GameObject firstValidUnityPrefab = GetFirstUnityPrefabFromValidUnityPrefabsCollection(validUnityPrefabs);
            PrefabCategory categoryWhichContainsPrefab = PrefabCategoryDatabase.Get().GetPrefabCategoryWhichContainsPrefab(firstValidUnityPrefab);

            if (categoryWhichContainsPrefab != null) AssignPrefabToDestinationBrushElement(categoryWhichContainsPrefab.GetPrefabByUnityPrefab(firstValidUnityPrefab));
            else
            {
                Prefab firstValidPrefab = GetFirstPrefabFromValidPrefabsCollection(PrefabFactory.Create(validUnityPrefabs));
                CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationBrushElement(firstValidPrefab);
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

        private void CreatePrefabToCategoryAssociationAndAssignPrefabToDestinationBrushElement(Prefab prefab)
        {
            PrefabWithPrefabCategoryAssociationQueue.Instance.Enqueue(PrefabWithPrefabCategoryAssociationFactory.Create(prefab, _destinationDecorPaintBrushElement.ParentBrush.DestinationCategoryForElementPrefabs));
            AssignPrefabToDestinationBrushElement(prefab);
        }

        private void AssignPrefabToDestinationBrushElement(Prefab prefab)
        {
            UndoEx.RecordForToolAction(_destinationDecorPaintBrushElement);
            _destinationDecorPaintBrushElement.Prefab = prefab;
        }
        #endregion
    }
}
#endif