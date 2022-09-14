#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectMoveGizmo : ObjectGizmo
    {
        #region Public Methods
        public override GizmoType GetGizmoType()
        {
            return GizmoType.Move;
        }

        public override void RenderHandles(TransformGizmoPivotPoint transformPivotPoint)
        {
            if (CanTransformObjects())
            {
                Vector3 newGizmoWorldPosition = Handles.PositionHandle(_worldPosition, _worldRotation);
                if (newGizmoWorldPosition != _worldPosition)
                {
                    GameObjectExtensions.RecordObjectTransformsForUndo(_targetObjects);
                    List<GameObject> topParents = GameObjectExtensions.GetParents(_targetObjects);

                    MoveObjects(newGizmoWorldPosition, topParents);

                    UndoEx.RecordForToolAction(this);
                    _worldPosition = newGizmoWorldPosition;
                    GizmoTransformedObjectsMessage.SendToInterestedListeners(this);
                }
            }
        }
        #endregion

        #region Private Methods
        private void MoveObjects(Vector3 newGizmoWorldPosition, List<GameObject> gameObjectsToMove)
        {
            foreach (GameObject gameObject in gameObjectsToMove)
            {
                Transform gameObjectTransform = gameObject.transform;
                gameObjectTransform.position = CalculateNewObjectPosition(newGizmoWorldPosition, gameObjectTransform);
            }
        }

        private Vector3 CalculateNewObjectPosition(Vector3 newGizmoWorldPosition, Transform gameObjectTransform)
        {
            Vector3 relationshipVector = gameObjectTransform.position - _worldPosition;
            return newGizmoWorldPosition + relationshipVector;
        }
        #endregion
    }
}
#endif