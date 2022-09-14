#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectScaleGizmo : ObjectGizmo
    {
        #region Private Constant Variables
        private const float _minObjectScaleValue = 0.00001f;
        #endregion

        #region Private Variables
        private Vector3 _accumScale = Vector3.one;
        private Dictionary<GameObject, Vector3> _objectScaleMap = new Dictionary<GameObject, Vector3>();
        #endregion

        #region Public Methods
        public override GizmoType GetGizmoType()
        {
            return GizmoType.Scale;
        }

        public override void RenderHandles(TransformGizmoPivotPoint transformPivotPoint)
        {
            if (Event.current.type == EventType.MouseDown)
            {
                _accumScale = Vector3.one;
                _objectScaleMap.Clear();
            }

            if (CanTransformObjects())
            {
                Vector3 newAccumScale = Handles.ScaleHandle(_accumScale, _worldPosition, _worldRotation, HandleUtility.GetHandleSize(_worldPosition));
                if (newAccumScale != _accumScale)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(_targetObjects);
                    List<GameObject> topParents = GameObjectExtensions.GetParents(_targetObjects);
                    if (_objectScaleMap.Count == 0)
                    {
                        foreach (var parent in topParents)
                            _objectScaleMap.Add(parent, parent.transform.localScale);
                    }

                    Vector3 scaleFactor = CalculateScaleFactorUsedToScaleObjects(newAccumScale);
                    ScaleObjectsBySpecifiedPivotPoint(transformPivotPoint, scaleFactor, topParents);

                    _accumScale = newAccumScale;
                    GizmoTransformedObjectsMessage.SendToInterestedListeners(this);
                }
            }
        }
        #endregion

        #region Private Methods
        private void ScaleObjectsBySpecifiedPivotPoint(TransformGizmoPivotPoint scalePivotPoint, Vector3 scaleFactor, List<GameObject> gameObjectsToScale)
        {
            if (scalePivotPoint == TransformGizmoPivotPoint.Pivot) ScaleObjectsByPivot(scaleFactor, gameObjectsToScale);
            else ScaleObjectsByGizmoPosition(scaleFactor, gameObjectsToScale);
        }

        private Vector3 CalculateScaleFactorUsedToScaleObjects(Vector3 newAccumScale)
        {
            Vector3 invScale = _accumScale.ReplaceCoordsValueWith(0.0f, 1.0f).GetInverse();
            return Vector3.Scale(newAccumScale, invScale);
        }

        private void ScaleObjectsByPivot(Vector3 scaleFactor, List<GameObject> gameObjectsToScale)
        {
            foreach (GameObject gameObject in gameObjectsToScale)
            {
                Transform gameObjectTransform = gameObject.transform;
                gameObjectTransform.localScale = CalculateNewObjectScale(gameObjectTransform, scaleFactor);
            }
        }

        private Vector3 CalculateNewObjectScale(Transform objectTransform, Vector3 scaleFactor)
        {
            Vector3 scale = objectTransform.localScale;
            Vector3 originalScale = _objectScaleMap[objectTransform.gameObject];
            if (scale.x == 0.0f) scale.x = originalScale.x;
            if (scale.y == 0.0f) scale.y = originalScale.y;
            if (scale.z == 0.0f) scale.z = originalScale.z;

            Vector3 newObjectScale = Vector3.Scale(scale, scaleFactor);
            newObjectScale.ReplaceCoordsValueWith(0.0f, _minObjectScaleValue);

            return newObjectScale;
        }

        private void ScaleObjectsByGizmoPosition(Vector3 scaleFactor, List<GameObject> gameObjectsToScale)
        {
            foreach (GameObject gameObject in gameObjectsToScale)
            {
                Transform gameObjectTransform = gameObject.transform;

                gameObjectTransform.localScale = CalculateNewObjectScale(gameObjectTransform, scaleFactor);
                ScaleObjectDistanceFromGizmoPosition(scaleFactor, gameObjectTransform);
            }
        }

        private void ScaleObjectDistanceFromGizmoPosition(Vector3 scaleFactor, Transform gameObjectTransform)
        {
            Vector3 gizmoToObject = gameObjectTransform.position - _worldPosition;
            gameObjectTransform.position = _worldPosition + Vector3.Scale(gizmoToObject, scaleFactor);
        }
        #endregion
    }
}
#endif