#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public abstract class ObjectGizmo : ScriptableObject
    {
        #region Protected Variables
        [SerializeField]
        protected Vector3 _worldPosition = Vector3.zero;
        [SerializeField]
        protected Quaternion _worldRotation = Quaternion.identity;

        protected IEnumerable<GameObject> _targetObjects;
        #endregion

        #region Public Properties
        public IEnumerable<GameObject> TargetObjects { set { _targetObjects = value; } }
        public Vector3 WorldPosition 
        { 
            get { return _worldPosition; } 
            set 
            {
                UndoEx.RecordForToolAction(this);
                _worldPosition = value; 
            } 
        }
        public Quaternion WorldRotation
        { 
            get { return _worldRotation; } 
            set 
            {
                UndoEx.RecordForToolAction(this);
                _worldRotation = value;
            } 
        }
        #endregion

        #region Public Abstract Methods
        public abstract GizmoType GetGizmoType();
        public abstract void RenderHandles(TransformGizmoPivotPoint transformPivotPoint);
        #endregion

        #region Protected Methods
        protected bool CanTransformObjects()
        {
            return _targetObjects != null;
        }
        #endregion
    }
}
#endif