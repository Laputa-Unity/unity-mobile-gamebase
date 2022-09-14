#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectQueries
    {
        #region Public Static Functions
        public static bool IntersectsAnyObjectsInScene(OrientedBox orientedBox, bool ignoreFaceTouch)
        {
            return GetIntersectingObjects(orientedBox, ignoreFaceTouch).Count != 0;
        }

        public static bool IntersectsAnyObjectsInScene(OrientedBox orientedBox, List<GameObject> ignoreObjects, bool ignoreFaceTouch)
        {
            return GetIntersectingObjects(orientedBox, ignoreObjects, ignoreFaceTouch).Count != 0;
        }

        public static List<GameObject> GetIntersectingObjects(OrientedBox orientedBox, bool ignoreFaceTouch)
        {
            List<GameObject> intersectingObjects = Octave3DScene.Get().OverlapBox(orientedBox);
            if (ignoreFaceTouch) intersectingObjects.RemoveAll(item => orientedBox.AreAllBoxPointsOnOrInFrontOfAnyFacePlane(item.GetWorldOrientedBox()));

            return intersectingObjects;
        }

        public static List<GameObject> GetIntersectingObjects(OrientedBox orientedBox, List<GameObject> ignoreObjects, bool ignoreFaceTouch)
        {
            List<GameObject> intersectedObjects = GetIntersectingObjects(orientedBox, ignoreFaceTouch);
            intersectedObjects.RemoveAll(item => ignoreObjects.Contains(item));

            return intersectedObjects;
        }

        public static Dictionary<GameObject, GameObject> GetPrefabToObjectConnectionMappings(List<GameObject> gameObjects)
        {
            if (gameObjects.Count == 0) return new Dictionary<GameObject, GameObject>();

            var prefabMappings = new Dictionary<GameObject, GameObject>(gameObjects.Count);
            foreach(GameObject gameObject in gameObjects)
            {
                GameObject connectedPrefab = gameObject.GetSourcePrefab();
                if (connectedPrefab != null && !prefabMappings.ContainsKey(connectedPrefab)) prefabMappings.Add(connectedPrefab, gameObject);
            }

            return prefabMappings;
        }

        public static bool IsGameObjectEmpty(GameObject gameObject)
        {
            if (gameObject.HasMesh()) return false;
            if (gameObject.HasTerrain()) return false;
            if (gameObject.HasLight()) return false;
            if (gameObject.HasParticleSystem()) return false;
            if (gameObject.HasSpriteRenderer()) return false;

            return true;
        }

        public static bool IsGameObjectHierarchyEmpty(GameObject rootObject)
        {
            List<GameObject> allObjectsInHierarchy = rootObject.GetAllChildrenIncludingSelf();
            foreach(GameObject gameObject in allObjectsInHierarchy)
            {
                if (!ObjectQueries.IsGameObjectEmpty(gameObject)) return false;
            }

            return true;
        }

        public static bool CanGameObjectBePickedByCursor(GameObject gameObject)
        {
            if (gameObject == null) return false;
            if (IsGameObjectEmpty(gameObject)) return false;
            return Octave3DWorldBuilder.ActiveInstance.IsWorkingObject(gameObject);
        }

        public static bool CanGameObjectBeInteractedWith(GameObject gameObject)
        {
            if (gameObject == null) return false;
            if (Octave3DWorldBuilder.ActiveInstance.IsPivotWorkingObject(gameObject)) return false;

            return Octave3DWorldBuilder.ActiveInstance.IsWorkingObject(gameObject);
        }

        public static bool IsGameObjectPartOfPlacementGuideHierarchy(GameObject gameObject)
        {
            return ObjectPlacementGuide.ExistsInScene && (ObjectPlacementGuide.Equals(gameObject) || ObjectPlacementGuide.ContainsChild(gameObject.transform));
        }

        public static bool ObjectHasOnlyEmptyChildren(GameObject rootObject)
        {
            List<GameObject> allChildren = rootObject.GetAllChildren();
            foreach (GameObject gameObject in allChildren)
            {
                if (!ObjectQueries.IsGameObjectEmpty(gameObject)) return false;
            }

            return true;
        }
        #endregion
    }
}
#endif