#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class CameraPotentialVisibleObjectsBox
    {
        #region Private Variables
        private OrientedBox _orientedBox = new OrientedBox();
        #endregion

        #region Public Properties
        public Vector3 Center { get { return _orientedBox.Center; } }
        public Vector3 HalfSize { get { return _orientedBox.ModelSpaceExtents; } }
        public Quaternion Rotation { get { return _orientedBox.Rotation; } }
        #endregion

        #region Public Methods
        public void Calculate(Camera camera, CameraViewVolume cameraViewVolume)
        {
            Transform cameraTransform = camera.transform;
            _orientedBox = new OrientedBox();

            _orientedBox.Rotation = cameraTransform.rotation;
            _orientedBox.Center = cameraTransform.position + cameraTransform.forward * cameraViewVolume.FarClipPlaneDistance * 0.5f;
            _orientedBox.ModelSpaceSize = new Vector3((cameraViewVolume.TopLeftPointOnFarPlane - cameraViewVolume.TopRightPointOnFarPlane).magnitude,
                                                      (cameraViewVolume.TopLeftPointOnFarPlane - cameraViewVolume.BottomLeftPointOnFarPlane).magnitude,
                                                      cameraViewVolume.FarClipPlaneDistance - cameraViewVolume.NearClipPlaneDistance);
        }
        #endregion
    }
}
#endif