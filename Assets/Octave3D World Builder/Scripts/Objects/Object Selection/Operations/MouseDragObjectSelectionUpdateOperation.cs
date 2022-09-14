#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class MouseDragObjectSelectionUpdateOperation : ObjectSelectionUpdateOperation
    {
        #region Private Variables
        private bool _mustAppendObjectsToSelection;
        private bool _mustDeselectObjects;
        #endregion

        #region Public Methods
        public override void Perform()
        {
            if (Event.current.InvolvesLeftMouseButton())
            {
                Event.current.DisableInSceneView();
        
                AcquireAppendAndDeselectStates();
                UpdateSelection();  
            }
        }
        #endregion

        #region Private Methods
        private void UpdateSelection()
        {
            List<GameObject> gameObjectsOverlappedBySelectionShape = ObjectSelection.Get().GetAllGameObjectsOverlappedBySelectionShape();
            
            if (!_mustAppendObjectsToSelection && !_mustDeselectObjects) UpdateSelectionForNoAppendAndNoDeselect(gameObjectsOverlappedBySelectionShape);
            else
            if (_mustAppendObjectsToSelection) AppendObjectsToSelection(gameObjectsOverlappedBySelectionShape);
            else
            if (_mustDeselectObjects) DeselectObjectsWithSelectionShape(gameObjectsOverlappedBySelectionShape);   
        }

        private void AcquireAppendAndDeselectStates()
        {
            _mustDeselectObjects = AllShortcutCombos.Instance.EnableDeselectObjectsWithSelectionShape.IsActive();

            // Note: We append only if:
            //  -the append shortcut is active;
            //  -the selection mode is set to 'Paint' and the user is not intending on deselecting objects with the object selection shape.
            _mustAppendObjectsToSelection = AllShortcutCombos.Instance.EnableAppendObjectsToSelection.IsActive() ||
                                            (ObjectSelectionSettings.Get().SelectionMode == ObjectSelectionMode.Paint && !_mustDeselectObjects);
        }

        private void UpdateSelectionForNoAppendAndNoDeselect(List<GameObject> gameObjectsOverlappedBySelectionShape)
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            ObjectSelectionUpdateMode _selectionUpdateMode = ObjectSelectionSettings.Get().SelectionUpdateMode;
           
            // If no object was overlapped by the selection shape, we can clear the selection. 
            // Note: We only clear the selection if the current number of selected objects is not 0. This
            //       allows us to avoid registering an Undo for nothing.
            if (gameObjectsOverlappedBySelectionShape.Count == 0 && objectSelection.NumberOfSelectedObjects != 0)
            {
                UndoEx.RecordForToolAction(objectSelection);
                objectSelection.Clear();
            }
            else
            // When the selection shape has overlapped objects, we must reset the selection by clearing it and selecting only
            // the objects which were overlapped.
            if (gameObjectsOverlappedBySelectionShape.Count != 0)
            {
                UndoEx.RecordForToolAction(objectSelection);
                objectSelection.Clear();

                if(_selectionUpdateMode == ObjectSelectionUpdateMode.EntireHierarchy) objectSelection.AddEntireGameObjectHierarchyToSelection(gameObjectsOverlappedBySelectionShape);
                else objectSelection.AddGameObjectCollectionToSelection(gameObjectsOverlappedBySelectionShape);
                objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingSelectionShape();
            }
        }

        private void AppendObjectsToSelection(List<GameObject> gameObjectsOverlappedBySelectionShape)
        {
            if (gameObjectsOverlappedBySelectionShape.Count != 0)
            {
                ObjectSelection objectSelection = ObjectSelection.Get();
                ObjectSelectionUpdateMode _selectionUpdateMode = ObjectSelectionSettings.Get().SelectionUpdateMode;

                // Note: We only continue if the current object selection is not the same as the
                //       collection of objects we are appending. If it is, we would be registering
                //       an unnecessary Undo operation.
                if (!objectSelection.IsSameAs(gameObjectsOverlappedBySelectionShape))
                {
                    if (_selectionUpdateMode == ObjectSelectionUpdateMode.EntireHierarchy)
                    {
                        UndoEx.RecordForToolAction(objectSelection);
                        objectSelection.AddEntireGameObjectHierarchyToSelection(gameObjectsOverlappedBySelectionShape);
                    }
                    else
                    {
                        UndoEx.RecordForToolAction(objectSelection);
                        objectSelection.AddGameObjectCollectionToSelection(gameObjectsOverlappedBySelectionShape);
                    }
                    objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingSelectionShape();
                }
            }
        }

        private void DeselectObjectsWithSelectionShape(List<GameObject> gameObjectsOverlappedBySelectionShape)
        {
            if (gameObjectsOverlappedBySelectionShape.Count != 0)
            {
                ObjectSelection objectSelection = ObjectSelection.Get();
                ObjectSelectionUpdateMode _selectionUpdateMode = ObjectSelectionSettings.Get().SelectionUpdateMode;

                if(_selectionUpdateMode == ObjectSelectionUpdateMode.EntireHierarchy)
                {
                    UndoEx.RecordForToolAction(objectSelection);
                    objectSelection.RemoveEntireGameObjectHierarchyFromSelection(gameObjectsOverlappedBySelectionShape);
                }
                else
                {
                    UndoEx.RecordForToolAction(objectSelection);
                    objectSelection.RemoveGameObjectCollectionFromSelection(gameObjectsOverlappedBySelectionShape);
                }
                objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingSelectionShape();
            }
        }
        #endregion
    }
}
#endif