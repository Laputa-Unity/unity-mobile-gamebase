#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionYOffsetVectorCalculator
    {
        #region Public Methods
        public Vector3 Calculate(ObjectPlacementPathTileConnectionTypeSettings tileConnectionTypeSettings, ObjectPlacementPath path)
        {
            return path.ExtensionPlane.normal * (tileConnectionTypeSettings.YOffset + path.Settings.ManualConstructionSettings.OffsetAlongGrowDirection);
        }
        #endregion
    }
}
#endif