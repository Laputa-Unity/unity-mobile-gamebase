#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectErase
    {
        #region Public Static Functions
        public static void EraseGameObjectCollection(List<GameObject> gameObjectsToErase)
        {
            List<GameObject> objectsToErase = GetGameObjectsWhichCanBeErased(gameObjectsToErase);
            PrepareGameObjectCollectionForEraseOperation(objectsToErase);
            foreach (GameObject gameObject in objectsToErase) EraseGameObject(gameObject);
        }

        public static void EraseObjectHierarchiesInObjectCollection(List<GameObject> gameObjectsToErase)
        {
            HashSet<GameObject> rootsToErase = new HashSet<GameObject>(Octave3DWorldBuilder.ActiveInstance.GetRoots(gameObjectsToErase));
            List<GameObject> objectsToErase = GetGameObjectsWhichCanBeErased(rootsToErase);
            PrepareGameObjectCollectionForEraseOperation(objectsToErase);
            foreach (GameObject gameObject in objectsToErase) EraseEntireGameObjectHierarchy(gameObject);
        }

        public static void EraseEntireGameObjectHierarchy(GameObject gameObject)
        {
            if (!CanGameObjectBeErased(gameObject)) return;
      
            ObjectEraserSettings eraserSettings = ObjectEraserSettings.Get();
            GameObject root = Octave3DWorldBuilder.ActiveInstance.GetRoot(gameObject);

            if (root != null) DestroyGameObject(root, eraserSettings.AllowUndoRedo);
        }

        public static void EraseGameObject(GameObject gameObject)
        {
            if (!CanGameObjectBeErased(gameObject)) return;
            
            // This function is buggy - it no longer works with newer versions of Unity
            // because erasing children in an object hierarchy is no longer allowed. Moving
            // children from one parent to another is also now restricted.
            ObjectEraserSettings eraserSettings = ObjectEraserSettings.Get();
            bool isPivotWorkingObject = Octave3DWorldBuilder.ActiveInstance.IsPivotWorkingObject(gameObject);

            if (isPivotWorkingObject)
            {
                List<GameObject> immediateChildren = gameObject.GetImmediateChildren();
                gameObject.MoveImmediateChildrenUpOneLevel(eraserSettings.AllowUndoRedo);
                DestroyGameObject(gameObject, eraserSettings.AllowUndoRedo);

                for (int childIndex = 0; childIndex < immediateChildren.Count; ++childIndex)
                {
                    if (ObjectQueries.IsGameObjectHierarchyEmpty(immediateChildren[childIndex]))
                        DestroyGameObject(immediateChildren[childIndex], eraserSettings.AllowUndoRedo);
                }
            }
            else
            {
                GameObject root = Octave3DWorldBuilder.ActiveInstance.GetRoot(gameObject);
                if (root == gameObject)
                {
                    List<GameObject> immediateChildren = gameObject.GetImmediateChildren();
                    gameObject.MoveImmediateChildrenUpOneLevel(eraserSettings.AllowUndoRedo);
                    DestroyGameObject(gameObject, eraserSettings.AllowUndoRedo);

                    foreach(GameObject child in immediateChildren)
                    {
                        if (ObjectQueries.IsGameObjectHierarchyEmpty(child)) DestroyGameObject(child, eraserSettings.AllowUndoRedo);
                    }
                }
                else
                {
                    GameObject immediateParent = gameObject.transform.parent.gameObject;

                    gameObject.MoveImmediateChildrenUpOneLevel(eraserSettings.AllowUndoRedo);
                    DestroyGameObject(gameObject, eraserSettings.AllowUndoRedo);

                    if (ObjectQueries.IsGameObjectHierarchyEmpty(root) && CanGameObjectBeErased(root))
                        DestroyGameObject(root, eraserSettings.AllowUndoRedo);
                    else
                    if (immediateParent.transform.childCount == 0 &&
                        ObjectQueries.IsGameObjectEmpty(immediateParent) &&
                        !Octave3DWorldBuilder.ActiveInstance.IsPivotWorkingObject(immediateParent)) DestroyGameObject(immediateParent, eraserSettings.AllowUndoRedo);
                }
            }
        }

        public static bool CanGameObjectBeErased(GameObject gameObject)
        {
            if (gameObject == null) return false;
            return ObjectQueries.CanGameObjectBeInteractedWith(gameObject) && !ObjectEraser.Get().EraseMask.IsGameObjectMasked(gameObject);
        }
        #endregion

        #region Private Static Functions
        private static void DestroyGameObject(GameObject gameObject, bool allowUndoRedo)
        {
            if (allowUndoRedo) UndoEx.DestroyObjectImmediate(gameObject);
            else Octave3DWorldBuilder.DestroyImmediate(gameObject);
        }

        private static List<GameObject> GetGameObjectsWhichCanBeErased(IEnumerable<GameObject> gameObjects)
        {
            var objectsWhichCanBeErased = new List<GameObject>();
            foreach (GameObject gameObject in gameObjects)
            {
                if (CanGameObjectBeErased(gameObject)) objectsWhichCanBeErased.Add(gameObject);
            }

            return objectsWhichCanBeErased;
        }

        private static void PrepareGameObjectCollectionForEraseOperation(IEnumerable<GameObject> gameObjects)
        {
            ObjectSelection objectSelection = ObjectSelection.Get();

            // Note: Before objects are erased, we have to remove them from the object selection.
            //       Otherwise, the selection information will be lost when performing an Undo
            //       operation. I can't figure out why this is :).
            foreach (GameObject gameObject in gameObjects)
            {
                objectSelection.RemoveGameObjectFromSelection(gameObject);
            }
        }
        #endregion
    }
}
#endif