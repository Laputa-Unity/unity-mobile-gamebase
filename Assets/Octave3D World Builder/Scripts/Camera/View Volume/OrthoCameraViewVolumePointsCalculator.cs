#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class OrthoCameraViewVolumePointsCalculator : CameraViewVolumePointsCalculator
    {
        #region Public Methods
        public override Vector3[] CalculateWorldSpaceVolumePoints(Camera camera)
        {
            // Cache needed data
            Transform cameraTransform = camera.transform;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 cameraRight = cameraTransform.right;
            Vector3 cameraUp = cameraTransform.up;
            Vector3 cameraLook = cameraTransform.forward;

            float halfVolumeVerticalSize = camera.orthographicSize;
            float halfVolumeHorizontalSize = halfVolumeVerticalSize * camera.aspect;        // Multiply by the aspect ratio to take the screen distortion into account

            // Calculate the volume points on the near plane
            Vector3[] worldSpaceVolumePoints = new Vector3[8];
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;

            // Calculate the volume points on the far plane
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;

            return worldSpaceVolumePoints;
        }
        #endregion
    }
}
#endif
