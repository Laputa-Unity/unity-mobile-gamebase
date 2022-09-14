#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class PerspectiveCameraViewVolumePointsCalculator : CameraViewVolumePointsCalculator
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

            float halfFovAngle = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float angleTangent = Mathf.Tan(halfFovAngle);

            float xToZRatio = angleTangent * camera.aspect;     // Multiply by the aspect ratio to take the screen distortion into account
            float yToZRatio = angleTangent;

            float xRatioMulNearPlane = xToZRatio * camera.nearClipPlane;
            float xRatioMulFarPlane = xToZRatio * camera.farClipPlane;
            float yRatioMulNearPlane = yToZRatio * camera.nearClipPlane;
            float yRatioMulFarPlane = yToZRatio * camera.farClipPlane;

            // Calculate the volume points on the near plane
            Vector3[] worldSpaceVolumePoints = new Vector3[8];
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * xRatioMulNearPlane + cameraUp * yRatioMulNearPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * xRatioMulNearPlane + cameraUp * yRatioMulNearPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * xRatioMulNearPlane - cameraUp * yRatioMulNearPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * xRatioMulNearPlane - cameraUp * yRatioMulNearPlane;

            // Calculate the volume points on the far plane
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * xRatioMulFarPlane + cameraUp * yRatioMulFarPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * xRatioMulFarPlane + cameraUp * yRatioMulFarPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * xRatioMulFarPlane - cameraUp * yRatioMulFarPlane;
            worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * xRatioMulFarPlane - cameraUp * yRatioMulFarPlane;

            return worldSpaceVolumePoints;
        }
        #endregion
    }
}
#endif
