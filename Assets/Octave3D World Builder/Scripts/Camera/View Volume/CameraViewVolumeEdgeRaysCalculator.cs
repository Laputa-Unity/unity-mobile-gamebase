#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    /// <summary>
    /// This class allows the client code to calculate the edge rays which connect the points
    /// of a camera view volume.
    /// </summary>
    public static class CameraViewVolumeEdgeRaysCalculator
    {
        #region Public Methods
        /// <summary>
        /// Calculates and returns the edge rays which connect the points of the specified camera
        /// view volume.
        /// </summary>
        public static Ray3D[] CalculateWorldSpaceVolumeEdgeRays(CameraViewVolume cameraViewVolume)
        {
            var worldSpaceVolumeEdgeRays = new Ray3D[12];

            // Calculate the rays that connect the points on the near plane
            worldSpaceVolumeEdgeRays[0] = new Ray3D(cameraViewVolume.TopLeftPointOnNearPlane, cameraViewVolume.TopRightPointOnNearPlane - cameraViewVolume.TopLeftPointOnNearPlane);
            worldSpaceVolumeEdgeRays[1] = new Ray3D(cameraViewVolume.TopRightPointOnNearPlane, cameraViewVolume.BottomRightPointOnNearPlane - cameraViewVolume.TopRightPointOnNearPlane);
            worldSpaceVolumeEdgeRays[2] = new Ray3D(cameraViewVolume.BottomRightPointOnNearPlane, cameraViewVolume.BottomLeftPointOnNearPlane - cameraViewVolume.BottomRightPointOnNearPlane);
            worldSpaceVolumeEdgeRays[3] = new Ray3D(cameraViewVolume.BottomLeftPointOnNearPlane, cameraViewVolume.TopLeftPointOnNearPlane - cameraViewVolume.BottomLeftPointOnNearPlane);

            // Calculate the rays that connect the points on the far plane
            worldSpaceVolumeEdgeRays[4] = new Ray3D(cameraViewVolume.TopLeftPointOnFarPlane, cameraViewVolume.TopRightPointOnFarPlane - cameraViewVolume.TopLeftPointOnFarPlane);
            worldSpaceVolumeEdgeRays[5] = new Ray3D(cameraViewVolume.TopRightPointOnFarPlane, cameraViewVolume.BottomRightPointOnFarPlane - cameraViewVolume.TopRightPointOnFarPlane);
            worldSpaceVolumeEdgeRays[6] = new Ray3D(cameraViewVolume.BottomRightPointOnFarPlane, cameraViewVolume.BottomLeftPointOnFarPlane - cameraViewVolume.BottomRightPointOnFarPlane);
            worldSpaceVolumeEdgeRays[7] = new Ray3D(cameraViewVolume.BottomLeftPointOnFarPlane, cameraViewVolume.TopLeftPointOnFarPlane - cameraViewVolume.BottomLeftPointOnFarPlane);

            // Calculate the rays that connect the points between the near and far planes
            worldSpaceVolumeEdgeRays[8] = new Ray3D(cameraViewVolume.TopLeftPointOnNearPlane, cameraViewVolume.TopLeftPointOnFarPlane - cameraViewVolume.TopLeftPointOnNearPlane);
            worldSpaceVolumeEdgeRays[9] = new Ray3D(cameraViewVolume.TopRightPointOnNearPlane, cameraViewVolume.TopRightPointOnFarPlane - cameraViewVolume.TopRightPointOnNearPlane);
            worldSpaceVolumeEdgeRays[10] = new Ray3D(cameraViewVolume.BottomRightPointOnNearPlane, cameraViewVolume.BottomRightPointOnFarPlane - cameraViewVolume.BottomRightPointOnNearPlane);
            worldSpaceVolumeEdgeRays[11] = new Ray3D(cameraViewVolume.BottomLeftPointOnNearPlane, cameraViewVolume.BottomLeftPointOnFarPlane - cameraViewVolume.BottomLeftPointOnNearPlane);

            return worldSpaceVolumeEdgeRays;
        }
        #endregion
    }
}
#endif
