#if UNITY_EDITOR
namespace O3DWB
{
    /// <summary>
    /// The members of this enum can be used to identify different points which
    /// make up a camera view volume.
    /// </summary>
    public enum CameraViewVolumePoint
    {
        TopLeftOnNearPlane = 0,
        TopRightOnNearPlane,
        BottomRightOnNearPlane,
        BottomLeftOnNearPlane,
        TopLeftOnFarPlane,
        TopRightOnFarPlane,
        BottomRightOnFarPlane,
        BottomLeftOnFarPlane
    }
}
#endif