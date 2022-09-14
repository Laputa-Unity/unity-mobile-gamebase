#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectInstantiation
    {
        #region Public Static Functions
        public static ObjectPlacementGuide InstantiateObjectPlacementGuide(Prefab prefab, string name)
        {
            GameObject guideObject = Octave3DWorldBuilder.Instantiate(prefab.UnityPrefab) as GameObject;

            guideObject.name = name;
            ObjectPlacementGuide objectPlacementGuide = guideObject.AddComponent<ObjectPlacementGuide>();
            guideObject.transform.parent = Octave3DWorldBuilder.ActiveInstance.transform;
 
            return objectPlacementGuide;
        }

        public static List<GameObject> InstantiateObjectHirarchiesFromPlacementDataCollection(List<ObjectPlacementData> objectPlacementDataCollection)
        {
            if (objectPlacementDataCollection.Count == 0) return new List<GameObject>();

            var instantiatedHierarchyRoots = new List<GameObject>(objectPlacementDataCollection.Count);
            foreach(ObjectPlacementData objectPlacementData in objectPlacementDataCollection)
            {
                instantiatedHierarchyRoots.Add(InstantiateObjectHierarchyFromPlacementData(objectPlacementData));
            }

            return instantiatedHierarchyRoots;
        }

        public static GameObject InstantiateObjectHierarchyFromPlacementData(ObjectPlacementData objectPlacementData)
        {
            return InstantiateObjectHierarchyFromPrefab(objectPlacementData.Prefab, objectPlacementData.WorldPosition, objectPlacementData.WorldRotation, objectPlacementData.WorldScale);
        }

        public static GameObject InstantiateObjectHierarchyFromPrefab(Prefab prefab, Transform transformData)
        {
            return InstantiateObjectHierarchyFromPrefab(prefab, transformData.position, transformData.rotation, transformData.lossyScale);
        }

        public static GameObject InstantiateObjectHierarchyFromPrefab(Prefab prefab, Vector3 worldPosition, Quaternion worldRotation, Vector3 worldScale)
        {
            return InstantiateObjectHierarchyFromPrefab(prefab.UnityPrefab, worldPosition, worldRotation, worldScale);
        }

        public static GameObject InstantiateObjectHierarchyFromPrefab(GameObject prefab, Vector3 worldPosition, Quaternion worldRotation, Vector3 worldScale)
        {
            GameObject instantiatedObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            UndoEx.RegisterCreatedGameObject(instantiatedObject);
            instantiatedObject.name = prefab.name;

            Transform objectTransform = instantiatedObject.transform;
            objectTransform.position = worldPosition;
            objectTransform.rotation = worldRotation;
            objectTransform.localScale = worldScale;

            SceneViewCamera.Instance.SetObjectVisibilityDirty();
            return instantiatedObject;
        }
        #endregion
    }
}
#endif