#if UNITY_EDITOR
namespace O3DWB
{
    public enum BoxPlaneClassificationResult
    {
        InFront = 0,
        Behind,
        Spanning,
        OnPlane     // Rare, but possible
    }
}
#endif