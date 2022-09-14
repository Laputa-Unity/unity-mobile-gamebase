#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionGizmos: ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _areGizmosActive = true;

        [SerializeField]
        private GizmoType _activeGizmoType = GizmoType.Move;
        [SerializeField]
        private TransformSpace _gizmoTransformSpace = TransformSpace.Global;
        [SerializeField]
        private TransformGizmoPivotPoint _gizmoTransformPivotPoint = TransformGizmoPivotPoint.Center;

        [SerializeField]
        private ObjectMoveGizmo _objectMoveGizmo;
        [SerializeField]
        private ObjectRotationGizmo _objectRotationGizmo;
        [SerializeField]
        private ObjectScaleGizmo _objectScaleGizmo;
        [SerializeField]
        private ObjectSelectionExtrudeGizmo _objectDuplicateGizmo;
        #endregion

        #region Public Properties
        public bool AreGizmosActive
        {
            get { return _areGizmosActive; }
            set
            {
                _areGizmosActive = value;
                SceneView.RepaintAll();
            }
        }

        public GizmoType ActiveGizmoType 
        {
            get { return _activeGizmoType; }
            set
            { 
                _activeGizmoType = value;
                AdjustActiveGizmoRotation();
                AdjustActiveGizmoPosition();

                SceneView.RepaintAll();
            } 
        }
        public TransformSpace GizmoTransformSpace 
        { 
            get { return _gizmoTransformSpace; }
            set 
            { 
                _gizmoTransformSpace = value;
                AdjustActiveGizmoRotation();

                SceneView.RepaintAll();
            } 
        }
        public TransformGizmoPivotPoint GizmoTransformPivotPoint 
        { 
            get { return _gizmoTransformPivotPoint; } 
            set 
            { 
                _gizmoTransformPivotPoint = value;
                AdjustActiveGizmoPosition();

                SceneView.RepaintAll();
            } 
        }
        public ObjectMoveGizmo ObjectMoveGizmo
        {
            get
            {
                if (_objectMoveGizmo == null) _objectMoveGizmo = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectMoveGizmo>();
                return _objectMoveGizmo;
            }
        }
        public ObjectRotationGizmo ObjectRotationGizmo
        {
            get
            {
                if (_objectRotationGizmo == null) _objectRotationGizmo = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectRotationGizmo>();
                return _objectRotationGizmo;
            }
        }
        public ObjectScaleGizmo ObjectScaleGizmo
        {
            get
            {
                if (_objectScaleGizmo == null) _objectScaleGizmo = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectScaleGizmo>();
                return _objectScaleGizmo;
            }
        }
        public ObjectSelectionExtrudeGizmo ObjectDuplicateGizmo
        {
            get
            {
                if (_objectDuplicateGizmo == null) _objectDuplicateGizmo = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectSelectionExtrudeGizmo>();
                return _objectDuplicateGizmo;
            }
        }
        #endregion

        #region Public Methods
        public bool OwnsGizmo(ObjectGizmo objectTransformGizmo)
        {
            if (objectTransformGizmo == ObjectMoveGizmo) return true;
            if (objectTransformGizmo == ObjectRotationGizmo) return true;
            if (objectTransformGizmo == ObjectScaleGizmo) return true;
            if (objectTransformGizmo == ObjectDuplicateGizmo) return true;
            return false;
        }

        public void RenderHandles(HashSet<GameObject> selectedGameObjects)
        {
            if (selectedGameObjects == null || selectedGameObjects.Count == 0 || !_areGizmosActive) return;

            ObjectGizmo activeGizmo = GetActiveGizmo();
            if (activeGizmo != null)
            {
                activeGizmo.TargetObjects = selectedGameObjects;
                activeGizmo.RenderHandles(_gizmoTransformPivotPoint);
            }
        }

        public void OnObjectSelectionUpdatedUsingMouseClick()
        {
            ObjectGizmo activeGizmo = GetActiveGizmo();
            if (activeGizmo != null)
            {
                /*GameObject lastSelectedGameObject = ObjectSelection.Get().GetLastSelectedGameObject();
                if (lastSelectedGameObject == null) return;
                Transform lastSelectedObjectTransform = lastSelectedGameObject.transform;

                if (_gizmoTransformPivotPoint == TransformGizmoPivotPoint.Pivot && lastSelectedGameObject != null 
                    && _activeGizmoType != GizmoType.Duplicate) activeGizmo.WorldPosition = lastSelectedObjectTransform.position;
                else activeGizmo.WorldPosition = ObjectSelection.Get().GetWorldCenter();

                if (_gizmoTransformSpace == TransformSpace.Local && lastSelectedGameObject != null
                    && _activeGizmoType != GizmoType.Duplicate) activeGizmo.WorldRotation = lastSelectedObjectTransform.rotation;
                else activeGizmo.WorldRotation = Quaternion.identity;*/

                if (ObjectSelection.Get().NumberOfSelectedObjects != 0)
                {
                    AdjustActiveGizmoPosition();
                    AdjustActiveGizmoRotation();
                }
            }
        }

        public void OnObjectSelectionUpdatedUsingSelectionShape()
        {
            ObjectGizmo activeGizmo = GetActiveGizmo();
            if (activeGizmo != null)
            {
                /* GameObject firstSelectedGameObject = ObjectSelection.Get().GetFirstSelectedGameObject();
                 if (firstSelectedGameObject == null) return;
                 Transform firstSelectedObjectTransform = firstSelectedGameObject.transform;

                 if (_gizmoTransformPivotPoint == TransformGizmoPivotPoint.Pivot && firstSelectedGameObject != null
                     && _activeGizmoType != GizmoType.Duplicate) activeGizmo.WorldPosition = firstSelectedObjectTransform.position;           
                 else activeGizmo.WorldPosition = ObjectSelection.Get().GetWorldCenter();

                 if (_gizmoTransformSpace == TransformSpace.Local && firstSelectedObjectTransform != null
                     && _activeGizmoType != GizmoType.Duplicate) activeGizmo.WorldRotation = firstSelectedObjectTransform.rotation;
                 else activeGizmo.WorldRotation = Quaternion.identity;*/

                if (ObjectSelection.Get().NumberOfSelectedObjects != 0)
                {
                    AdjustActiveGizmoPosition();
                    AdjustActiveGizmoRotation();
                }
            }
        }

        public void OnObjectSelectionUpdated()
        {
            OnObjectSelectionUpdatedUsingSelectionShape();
        }
        #endregion

        #region Private Methods
        private ObjectGizmo GetActiveGizmo()
        {
            if (_activeGizmoType == GizmoType.Move) return ObjectMoveGizmo;
            if (_activeGizmoType == GizmoType.Rotate) return ObjectRotationGizmo;
            if (_activeGizmoType == GizmoType.Scale) return ObjectScaleGizmo;
            if (_activeGizmoType == GizmoType.Duplicate) return ObjectDuplicateGizmo;
            return null;
        }

        private void AdjustActiveGizmoPosition()
        {
            ObjectGizmo activeGizmo = GetActiveGizmo();
            if (activeGizmo != null)
            {
                if (_gizmoTransformPivotPoint == TransformGizmoPivotPoint.Pivot && _activeGizmoType != GizmoType.Duplicate)
                {
                    GameObject firstSelectedObject = ObjectSelection.Get().GetFirstSelectedGameObject();
                    if (firstSelectedObject != null) activeGizmo.WorldPosition = firstSelectedObject.transform.position;
                }
                else activeGizmo.WorldPosition = ObjectSelection.Get().GetWorldCenter();
            }
        }

        private void AdjustActiveGizmoRotation()
        {
            ObjectGizmo activeGizmo = GetActiveGizmo();
            if (activeGizmo != null)
            {
                if ((_gizmoTransformSpace == TransformSpace.Local && _activeGizmoType != GizmoType.Duplicate) || _activeGizmoType == GizmoType.Scale)
                {
                    GameObject firstSelectedObject = ObjectSelection.Get().GetFirstSelectedGameObject();
                    if (firstSelectedObject != null) activeGizmo.WorldRotation = firstSelectedObject.transform.rotation;
                    else activeGizmo.WorldRotation = Quaternion.identity;
                }
                else activeGizmo.WorldRotation = Quaternion.identity;
            }
        }
        #endregion
    }
}
#endif