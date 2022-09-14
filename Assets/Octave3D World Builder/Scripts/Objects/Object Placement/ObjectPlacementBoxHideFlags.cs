#if UNITY_EDITOR
using System;

namespace O3DWB
{
    [Flags]
    public enum ObjectPlacementBoxHideFlags
    {
        None = 0x0,

        PathExcludeCorners = 0x1,
        PathApplyBorders = 0x2,

        BlockExcludeCorners = 0x4,
        BlockApplySubdivisions = 0x8
    }
}
#endif