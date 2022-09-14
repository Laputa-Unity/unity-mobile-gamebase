#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ClickObjectSelectionUpdateOperation : ObjectSelectionUpdateOperation
    {
        #region Public Methods
        public override void Perform()
        {
            if (Event.current.InvolvesLeftMouseButton())
            {
                Event.current.DisableInSceneView();

                MouseCursorRayHit cursorRayHit = ObjectSelection.Get().GetObjectPickedByCursor();
                if (cursorRayHit.WasAnObjectHit)
                {
                    GameObject closestHitObject = cursorRayHit.SortedObjectRayHits[0].HitObject;
                    if (cursorRayHit.WasLightObjectHit || cursorRayHit.WasParticleSystemHit) closestHitObject = cursorRayHit.GetClosestHitParticleSystemOrLightObject();
                 
                    // Note: If append mode is enabled, we will toggle the selected state of the picked object. Otherwise,
                    //       we will clear the current selection and select only the picked game object.
                    if (AllShortcutCombos.Instance.EnableAppendObjectsToSelection.IsActive()) ToggleObjectSelectedState(closestHitObject);
                    else ClearSelectionAndSelectObject(closestHitObject);

                    if(Event.current.clickCount == 2)
                    {
                        UndoEx.RecordForToolAction(ObjectSelection.Get());
                        ObjectSelectionActions.SelectAllObjectsWithSamePrefabAsObject(closestHitObject);
                        ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingMouseClick();
                    }
                }
                else ClearSelectionWhenNoObjectPicked();
            }
        }
        #endregion

        #region Private Methods
        private void ToggleObjectSelectedState(GameObject gameObject)
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            ObjectSelectionUpdateMode selectionUpdateMode = ObjectSelectionSettings.Get().SelectionUpdateMode;

            UndoEx.RecordForToolAction(objectSelection);

            if(selectionUpdateMode == ObjectSelectionUpdateMode.EntireHierarchy)
            {
                if (objectSelection.IsGameObjectSelected(gameObject)) objectSelection.RemoveEntireGameObjectHierarchyFromSelection(gameObject);
                else objectSelection.AddEntireGameObjectHierarchyToSelection(gameObject);
            }
            else
            {
                if (objectSelection.IsGameObjectSelected(gameObject)) objectSelection.RemoveGameObjectFromSelection(gameObject);
                else objectSelection.AddGameObjectToSelection(gameObject);
            }
            objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingMouseClick();
        }

        private void ClearSelectionAndSelectObject(GameObject gameObject)
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            ObjectSelectionUpdateMode selectionUpdateMode = ObjectSelectionSettings.Get().SelectionUpdateMode;

            if (selectionUpdateMode == ObjectSelectionUpdateMode.EntireHierarchy)
            {
                UndoEx.RecordForToolAction(objectSelection);
                objectSelection.Clear();
                objectSelection.AddEntireGameObjectHierarchyToSelection(gameObject);
                objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingMouseClick();
            }
            else
            {
                // Note: We only continue if the picked object is not the only currently selected object. If it is, it means
                //       we would be registering an Undo operation for nothing.
                if (!ObjectSelection.Get().IsSameAs(gameObject))
                {
                    UndoEx.RecordForToolAction(objectSelection);
                    objectSelection.Clear();
                    objectSelection.AddGameObjectToSelection(gameObject);
                    objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdatedUsingMouseClick();
                }
            }
        }

        private void ClearSelectionWhenNoObjectPicked()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
    
            // When no object is picked, we will clear the selection.
            // Note: We only clear the selection if:
            //  -the current number of selected objects is not 0. This allows us to avoid registering
            //   an Undo operation for nothing;
            //  -append mode is not active. If it is, the user may have clicked an empty area in order
            //   to start selecting multiple objects with the selection shape. In this case we don't
            //   want to clear the selection because otherwise, there would be no way for the user to
            //   append objects to the selection using the selection shape;
            //  -deselect objects with selection shape mode is not active. If it is, clearing the object
            //   selection would not allow the user to deselect only the objects they desire because when
            //   they click in an empty space all objects would already be deselected.
            if (objectSelection.NumberOfSelectedObjects != 0 &&
                !AllShortcutCombos.Instance.EnableAppendObjectsToSelection.IsActive() &&
                !AllShortcutCombos.Instance.EnableDeselectObjectsWithSelectionShape.IsActive())
            {
                UndoEx.RecordForToolAction(objectSelection);
                objectSelection.Clear();
            }
        }
        #endregion
    }
}
#endif