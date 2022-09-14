#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class CameraPotentialVisibleObjectsSphere
    {
        #region Private Variables
        private float _radius;
        private Vector3 _center;
        #endregion

        #region Public Properties
        public float Radius { get { return _radius; } }
        public Vector3 Center { get { return _center; } }
        public Sphere Sphere { get { return new Sphere(_center, _radius); } }
        #endregion

        #region Public Methods
        public void Calculate(Camera camera, CameraViewVolume cameraViewVolume)
        {
            Transform cameraTransform = camera.transform;

            _center = cameraTransform.position + cameraTransform.forward * cameraViewVolume.FarClipPlaneDistance * 0.5f;
            _radius = (cameraViewVolume.TopLeftPointOnFarPlane - _center).magnitude * 1.01f;    
        }
        #endregion
    }
}
#endif