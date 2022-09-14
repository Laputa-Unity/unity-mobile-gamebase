#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectActions
    {
        #region Public Static Functions 
        public static void AssignObjectsToGroup(IEnumerable<GameObject> gameObjects, ObjectGroup objectGroup)
        {
            var roots = Octave3DWorldBuilder.ActiveInstance.GetRoots(gameObjects);
            foreach(var root in roots)
            {
                UndoEx.SetTransformParent(root.transform, objectGroup.GroupObject.transform);
            }
        }

        public static List<GameObject> Duplicate(IEnumerable<GameObject> sourceObjects)
        {
            List<GameObject> sourceParents = Octave3DWorldBuilder.ActiveInstance.GetRoots(sourceObjects);

            var clonedObjects = new List<GameObject>();
            foreach (GameObject parent in sourceParents)
            {
                GameObject prefab = parent.GetSourcePrefab();
                Transform parentTransform = parent.transform;

                if (prefab == null)
                {
                    GameObject clonedParent = parent.CloneAsWorkingObject(parentTransform.parent);
                    //clonedParent.transform.parent = parent.transform.parent;
                    clonedObjects.AddRange(clonedParent.GetAllChildrenIncludingSelf());
                    Octave3DScene.Get().RegisterObjectHierarchy(clonedParent);
                }
                else
                {
                    GameObject clonedParent = ObjectInstantiation.InstantiateObjectHierarchyFromPrefab(prefab, parentTransform.position, parentTransform.rotation, parentTransform.lossyScale);
                    clonedObjects.AddRange(clonedParent.GetAllChildrenIncludingSelf());
                    Octave3DScene.Get().RegisterObjectHierarchy(clonedParent);
                }
            }

            SceneViewCamera.Instance.SetObjectVisibilityDirty();
            return clonedObjects;
        }

        public static List<GameObject> ReplaceGameObjectHierarchyCollectionPrefab(List<GameObject> gameObjectCollection, GameObject newPrefab)
        {
            var newObjects = new List<GameObject>();

            List<GameObject> roots = Octave3DWorldBuilder.ActiveInstance.GetRoots(gameObjectCollection);
            foreach (GameObject gameObject in roots)
            {
                GameObject newObject = ReplaceGameObjectHierarchyPrefab(gameObject, newPrefab);
                if (newObject != null) newObjects.Add(newObject);
            }

            return newObjects;
        }

        public static GameObject ReplaceGameObjectHierarchyPrefab(GameObject gameObject, GameObject newPrefab)
        {
            if (gameObject == null) return null;

            var prefabType = PrefabUtility.GetPrefabAssetType(newPrefab);
            if (prefabType == PrefabAssetType.NotAPrefab || prefabType == PrefabAssetType.Model) return null;

            // Store any needed object data
            OrientedBox originalWorldOrientedBox = gameObject.GetHierarchyWorldOrientedBox();
            if (originalWorldOrientedBox.IsInvalid()) return null;
            int originalObjectLayer = gameObject.layer;
            bool isObjectStatic = gameObject.isStatic;

            Transform originalObjectTransform = gameObject.transform;
            Vector3 worldScale = originalObjectTransform.lossyScale;
            Quaternion worldRotation = originalObjectTransform.rotation;

            // Create a new game object from the specified prefab
            GameObject newObject = PrefabUtility.InstantiatePrefab(newPrefab) as GameObject;
            if (newObject != null)
            {
                // Register the created object for Undo and set its transform data. Also store any significant
                // data that the original object had before it was destroyed.
                UndoEx.RegisterCreatedGameObject(newObject);
                Transform newObjectTransform = newObject.transform;
                newObjectTransform.localScale = worldScale;
                newObjectTransform.rotation = worldRotation;
                newObjectTransform.parent = originalObjectTransform.transform.parent;
                newObject.layer = originalObjectLayer;
                newObject.isStatic = isObjectStatic;
                 
                // We will adjust the new object's position such that its center is the same as the
                // one the original object had. This produces better results especially when the new 
                // object is a multi-level object hierarchy.
                OrientedBox newHierarchyWorldOrientedBox = newObject.GetHierarchyWorldOrientedBox();
                if(newHierarchyWorldOrientedBox.IsInvalid())
                {
                    GameObject.DestroyImmediate(newObject);
                    return null;
                }
                newObjectTransform.position = ObjectPositionCalculator.CalculateObjectHierarchyPosition(newPrefab, originalWorldOrientedBox.Center, worldScale, worldRotation);

                // We will also inform the scene that a new object was created so that all the necessary steps can be performed
                Octave3DScene.Get().RegisterObjectHierarchy(newObject);

                // Destroy the old object
                UndoEx.DestroyObjectImmediate(gameObject);

                return newObject;
            }

            return null;
        }

        public static void HideObjects(List<GameObject> gameObjects)
        {
            foreach(GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(false);
            }
        }

        public static void ShowObjects(List<GameObject> gameObjects)
        {
            foreach(GameObject gameObject in gameObjects)
            {
                gameObject.SetActive(true);
            }
        }

        public static void MakeObjectsStatic(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.isStatic = true;
            }
        }

        public static void MakeObjectsDynamic(List<GameObject> gameObjects)
        {
            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.isStatic = false;
            }
        }

        public static void EraseGameObjectsInAllLayers()
        {
            List<GameObject> allGameObjectsInAllLayers = ObjectLayerDatabase.Get().GetAllGameObjectsInAllLayers();
            ObjectErase.EraseGameObjectCollection(allGameObjectsInAllLayers);
        }

        public static void EraseAllGameObjectsInLayer(int objectLayer)
        {
            List<GameObject> allGameObjectsInLayer = ObjectLayerDatabase.Get().GetAllGameObjectsInLayer(objectLayer);
            ObjectErase.EraseGameObjectCollection(allGameObjectsInLayer);
        }
        #endregion
    }
}
#endif