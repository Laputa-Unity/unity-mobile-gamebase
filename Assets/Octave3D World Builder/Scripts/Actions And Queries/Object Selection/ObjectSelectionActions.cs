#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectSelectionActions
    {
        #region Public Static Functions
        public static void AlignSelectionToAxis(Axis axis)
        {
            int axisIndex = (int)axis;

            List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            List<GameObject> selectedParents = GameObjectExtensions.GetParents(allSelectedObjects);
            if (selectedParents.Count == 0) return;

            float average = 0.0f;
            foreach(var parent in selectedParents)
            {
                average += parent.transform.position[axisIndex];
            }
            average /= selectedParents.Count;

            GameObjectExtensions.RecordObjectTransformsForUndo(selectedParents);
            foreach (var parent in selectedParents)
            {
                Transform parentTransform = parent.transform;
                Vector3 alignedPosition = parentTransform.position;
                alignedPosition[axisIndex] = average;

                parentTransform.position = alignedPosition;
            }
        }

        public static void DuplicateSelection()
        {
            if (ObjectSelection.Get().NumberOfSelectedObjects == 0) return;

            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> allSelectedObjects = objectSelection.GetAllSelectedGameObjects();
            List<GameObject> selectedParents = GameObjectExtensions.GetParents(allSelectedObjects);

            var clonedObjects = new List<GameObject>();
            var clonedParents = new List<GameObject>();
            foreach(GameObject parent in selectedParents)
            {
                GameObject prefab = parent.GetSourcePrefab();
                Transform parentTransform = parent.transform;

                if(prefab == null)
                {
                    GameObject clonedParent = parent.CloneAsWorkingObject(parentTransform.parent);
                    //clonedParent.transform.parent = parent.transform.parent;
                    clonedObjects.AddRange(clonedParent.GetAllChildrenIncludingSelf());
                    Octave3DScene.Get().RegisterObjectHierarchy(clonedParent);
                    clonedParents.Add(clonedParent);
                }
                else
                {
                    GameObject clonedParent = ObjectInstantiation.InstantiateObjectHierarchyFromPrefab(prefab, parentTransform.position, parentTransform.rotation, parentTransform.lossyScale);
                    clonedObjects.AddRange(clonedParent.GetAllChildrenIncludingSelf());
                    Octave3DScene.Get().RegisterObjectHierarchy(clonedParent);
                    clonedParents.Add(clonedParent);
                }
            }

            if(clonedObjects.Count != 0)
            {
                objectSelection.Clear();
                objectSelection.AddGameObjectCollectionToSelection(clonedObjects);
                objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdated();
            }

            if(clonedParents.Count != 0)
            {
                ObjectHierarchyRootsWerePlacedInSceneMessage.SendToInterestedListeners(clonedParents, ObjectHierarchyRootsWerePlacedInSceneMessage.PlacementType.Selection);
            }

            SceneViewCamera.Instance.SetObjectVisibilityDirty();
        }

        public static void InvertSelection()
        {
            List<GameObject> allWorkingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
            List<GameObject> objectsToSelect = new List<GameObject>();
            ObjectSelection objectSelection = ObjectSelection.Get();

            foreach(var gameObject in allWorkingObjects)
            {
                if (!objectSelection.IsGameObjectSelected(gameObject)) objectsToSelect.Add(gameObject);
            }

            objectSelection.Clear();
            objectSelection.AddGameObjectCollectionToSelection(objectsToSelect);
            objectSelection.ObjectSelectionGizmos.OnObjectSelectionUpdated();
        }

        public static List<GameObject> ReplaceSelectedObjectsPrefabOnMouseClick()
        {
            MouseCursorRayHit cursorRayHit = ObjectSelection.Get().GetObjectPickedByCursor();
            if (cursorRayHit.WasAnObjectHit && !ObjectSelection.Get().IsGameObjectSelected(cursorRayHit.ClosestObjectRayHit.HitObject))
            {
                GameObject hitObject = cursorRayHit.ClosestObjectRayHit.HitObject;
                hitObject = Octave3DWorldBuilder.ActiveInstance.GetRoot(hitObject);
                if (hitObject == null) return new List<GameObject>();

                GameObject newPrefabForSelectedObjects = hitObject.GetSourcePrefab();
                if (newPrefabForSelectedObjects == null)
                {
                    List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                    ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allSelectedObjects);

                    List<GameObject> selectedRoots = Octave3DWorldBuilder.ActiveInstance.GetRoots(allSelectedObjects);
                    var newObjects = new List<GameObject>();
                    foreach(var root in selectedRoots)
                    {
                        Transform rootTransform = root.transform;
                        GameObject newObject = GameObjectExtensions.CloneAsWorkingObject(hitObject, hitObject.transform.parent, true);
                        if(newObject != null)
                        {
                            Transform objectTransform = newObject.transform;
                            objectTransform.position = rootTransform.position;
                            objectTransform.rotation = rootTransform.rotation;
                            objectTransform.SetWorldScale(rootTransform.lossyScale);

                            UndoEx.DestroyObjectImmediate(root);
                            newObjects.Add(newObject);
                        }
                    }

                    return newObjects;
                }
                else
                {
                    List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
                    ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allSelectedObjects);
                    List<GameObject> newObjects = ObjectActions.ReplaceGameObjectHierarchyCollectionPrefab(allSelectedObjects, newPrefabForSelectedObjects);
                    newObjects.RemoveAll(item => item == null);

                    return newObjects;
                }
            }

            return new List<GameObject>();
        }

        public static List<GameObject> ReplaceSelectedObjectsWithPrefab(GameObject prefab)
        {
            List<GameObject> allSelectedObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allSelectedObjects);
            List<GameObject> newObjects = ObjectActions.ReplaceGameObjectHierarchyCollectionPrefab(allSelectedObjects, prefab);
            newObjects.RemoveAll(item => item == null);

            return newObjects;
        }

        public static void SelectAllObjectsWithSamePrefabAsCurrentSelection()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> allSelectedObjects = objectSelection.GetAllSelectedGameObjects();
            Dictionary<GameObject, GameObject> prefabToObjectMappings = ObjectQueries.GetPrefabToObjectConnectionMappings(allSelectedObjects);

            List<GameObject> workingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
            foreach(GameObject gameObject in workingObjects)
            {
                GameObject sourcePrefab = gameObject.GetSourcePrefab();
                if(sourcePrefab != null)
                {
                    if (prefabToObjectMappings.ContainsKey(sourcePrefab)) objectSelection.AddGameObjectToSelection(gameObject);
                }
            }
        }

        public static void SelectAllObjectsWithSamePrefabAsObject(GameObject gameObject)
        {
            GameObject root = Octave3DWorldBuilder.ActiveInstance.GetRoot(gameObject);
            if (root == null) return;

            GameObject sourcePrefab = root.GetSourcePrefab();
            if (sourcePrefab == null) return;

            ObjectSelection objectSelection = ObjectSelection.Get();
            List<GameObject> workingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();

            foreach (GameObject workingObject in workingObjects)
            {
                GameObject workingObjectRoot = Octave3DWorldBuilder.ActiveInstance.GetRoot(workingObject);
                if (workingObjectRoot == null) continue;

                GameObject workingObjectSourcePrefab = workingObjectRoot.GetSourcePrefab();
                if (workingObjectSourcePrefab != null && 
                    workingObjectSourcePrefab == sourcePrefab)
                {
                    objectSelection.AddGameObjectCollectionToSelection(workingObjectRoot.GetAllChildrenIncludingSelf());
                }
            }
        }

        public static void AssignSelectedObjectsToLayer(int objectLayer)
        {
            List<GameObject> allSelectedGameObjects = ObjectSelection.Get().GetAllSelectedGameObjects();
            ObjectLayerDatabase.Get().AssignObjectsToLayer(allSelectedGameObjects, objectLayer);
        }

        public static void SelectAllObjectsInActiveLayer()
        {
            SelectAllObjectsInLayer(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void SelectAllObjectsInAllLayers()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.Clear();

            List<int> allLayers = ObjectLayerDatabase.Get().GetAllObjectLayers();
            foreach (int objectLayer in allLayers)
            {
                objectSelection.AddGameObjectCollectionToSelection(ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer));
            }
            ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();
        }

        public static void SelectAllObjectsInLayer(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);

            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.Clear();
            objectSelection.AddGameObjectCollectionToSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();
        }

        public static void DeselectAllObjectsInActiveLayer()
        {
            DeselectAllObjectsInLayer(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void DeselectAllObjectsInAllLayers()
        {
            List<int> allLayers = ObjectLayerDatabase.Get().GetAllObjectLayers();
            foreach (int objectLayer in allLayers)
            {
                DeselectAllObjectsInLayer(objectLayer);
            }
        }

        public static void AppendAllObjectsInActiveLayerToSelection()
        {
            AppendAllObjectsInLayerToSelection(ObjectLayerDatabase.Get().ActiveLayer);
        }

        public static void AppendAllObjectsInLayerToSelection(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);

            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.AddGameObjectCollectionToSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();
        }

        public static void DeselectAllObjectsInLayer(int objectLayer)
        {
            List<GameObject> allObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);
            ObjectSelection.Get().RemoveGameObjectCollectionFromSelection(allObjectsInLayer);
            ObjectSelection.Get().ObjectSelectionGizmos.OnObjectSelectionUpdated();
        }
        #endregion
    }
}
#endif