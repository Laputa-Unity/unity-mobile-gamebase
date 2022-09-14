#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class UndoEx
    {
        #region Public Static Functions
        public static void RecordForToolAction(UnityEngine.Object objectToRecord)
        {
            Undo.RecordObject(objectToRecord, UndoActionNames.ToolActionName);
        }

        public static void RecordForToolAction<T>(List<T> objectsToRecord) where T : UnityEngine.Object
        {
            foreach (UnityEngine.Object objectToRecord in objectsToRecord)
            {
                Undo.RecordObject(objectToRecord, UndoActionNames.ToolActionName);
            }
        }

        public static void IncrementCurrentGroup()
        {
            Undo.IncrementCurrentGroup();
        }

        public static void DestroyObjectImmediate(UnityEngine.Object gameObject)
        {
            Undo.DestroyObjectImmediate(gameObject);
        }

        public static void SetTransformParent(Transform transform, Transform newParentTransform)
        {
            if (transform.parent != newParentTransform)
                Undo.SetTransformParent(transform, newParentTransform, UndoActionNames.ToolActionName);
        }

        public static T AddComponent<T>(GameObject gameObject) where T : Component
        {
            return Undo.AddComponent<T>(gameObject);
        }

        public static void RegisterCreatedGameObject(GameObject gameObject)
        {
            Undo.RegisterCreatedObjectUndo(gameObject, UndoActionNames.ToolActionName);
        }

        public static void RegisterCreatedScriptableObject(ScriptableObject scriptableObject)
        {
            Undo.RegisterCreatedObjectUndo(scriptableObject, UndoActionNames.ToolActionName);
        }

        public static void RecordStateForGameObjects(List<GameObject> gameObjectsToRecord)
        {
            Undo.RecordObjects(gameObjectsToRecord.ToArray(), UndoActionNames.ToolActionName);
        }
        #endregion
    }
}
#endif