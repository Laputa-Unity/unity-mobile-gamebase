#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectRotationGizmo : ObjectGizmo
    {
        #region Public Methods
        public override GizmoType GetGizmoType()
        {
            return GizmoType.Rotate;
        }

        public override void RenderHandles(TransformGizmoPivotPoint transformPivotPoint)
        {
            if (CanTransformObjects())
            {
                Quaternion newWorldRotationAccumulatedByGizmoInteraction = Handles.RotationHandle(_worldRotation, _worldPosition);
                if (newWorldRotationAccumulatedByGizmoInteraction.eulerAngles != _worldRotation.eulerAngles)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(_targetObjects);
                    List<GameObject> topParents = GameObjectExtensions.GetParents(_targetObjects);

                    Quaternion rotationAmount = CalculateRotationAmount(newWorldRotationAccumulatedByGizmoInteraction);
                    RotateObjectsAroundSpecifiedPivotPoint(transformPivotPoint, rotationAmount, topParents);

                    UndoEx.RecordForToolAction(this);
                    _worldRotation = newWorldRotationAccumulatedByGizmoInteraction;
                    GizmoTransformedObjectsMessage.SendToInterestedListeners(this);
                }
            }
        }
        #endregion

        #region Private Methods
        private void RotateObjectsAroundSpecifiedPivotPoint(TransformGizmoPivotPoint rotationPivotPoint, Quaternion rotationAmount, List<GameObject> gameObjectsToRotate)
        {
            if (rotationPivotPoint == TransformGizmoPivotPoint.Pivot) RotateObjectsAroundPivot(rotationAmount, gameObjectsToRotate);
            else RotateObjectsAroundGizmoPosition(rotationAmount, gameObjectsToRotate);
        }

        private Quaternion CalculateRotationAmount(Quaternion newAccumulatedRotationByGizmoInteraction)
        {
            Quaternion inverseGizmoWorldRotation = Quaternion.Inverse(_worldRotation);
            return newAccumulatedRotationByGizmoInteraction * inverseGizmoWorldRotation;
        }

        private void RotateObjectsAroundPivot(Quaternion rotationAmount, List<GameObject> gameObjectsToScale)
        {
            foreach (GameObject gameObject in gameObjectsToScale)
            {
                Transform gameObjectTransform = gameObject.transform;
                gameObjectTransform.rotation = rotationAmount * gameObjectTransform.rotation;
            }
        }

        private void RotateObjectsAroundGizmoPosition(Quaternion rotationAmount, List<GameObject> gameObjectsToScale)
        {
            foreach (GameObject gameObject in gameObjectsToScale)
            {
                Transform gameObjectTransform = gameObject.transform;

                gameObjectTransform.rotation = rotationAmount * gameObjectTransform.rotation;
                RotateObjectPositionAroundGizmoPosition(rotationAmount, gameObjectTransform);
            }
        }

        private void RotateObjectPositionAroundGizmoPosition(Quaternion rotationAmount, Transform gameObjectTransform)
        {
            Vector3 gizmoToObject = gameObjectTransform.position - _worldPosition;
            Vector3 rotatedGizmoToObject = rotationAmount * gizmoToObject;

            gameObjectTransform.position = _worldPosition + rotatedGizmoToObject;
        }
        #endregion
    }
}
#endif