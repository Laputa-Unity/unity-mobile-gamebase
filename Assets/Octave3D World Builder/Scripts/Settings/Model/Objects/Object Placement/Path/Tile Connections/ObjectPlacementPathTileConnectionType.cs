#if UNITY_EDITOR
namespace O3DWB
{
    public enum ObjectPlacementPathTileConnectionType
    {
        Begin = 0,
        End, 
        Forward,
        Turn,
        TJunction,
        Cross,
        Autofill
    }
}
#endif