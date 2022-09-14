#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class Mirror
    {
        #region Private Variables
        [SerializeField]
        private Vector3 _planeNormal = Vector3.right;
        [SerializeField]
        private float _planeDistanceFromOrigin = 0.0f;
        [SerializeField]
        private Vector3 _worldCenter;
        #endregion

        #region Public Properties
        public Plane WorldPlane { get { return new Plane(_planeNormal, _planeDistanceFromOrigin); } }
        public Vector3 WorldCenter { get { return _worldCenter; } }
        #endregion

        #region Constructors
        public Mirror()
        {
            _planeNormal = Vector3.right;
            _planeDistanceFromOrigin = 0.0f;
            _worldCenter = Vector3.zero;
        }
        #endregion

        #region Public Methods
        public void SetWorldPlaneNormal(Vector3 worldPlaneNormal)
        {
            worldPlaneNormal.Normalize();
            _planeNormal = worldPlaneNormal;
        }

        public void SetWorldCenter(Vector3 worldCenter)
        {
            _worldCenter = worldCenter;
            _planeDistanceFromOrigin = new Plane(_planeNormal, worldCenter).distance;
        }
        #endregion
    }
}
#endif