#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionScaleCalculator
    {
        #region Public Methods
        public Vector3 CalculateWorldScale(float tileConnectionXZSize, Vector3 tileConnectionPrefabBoxSize, Transform tileConnectionPrefabTransform, ObjectPlacementPath path)
        {
            if(path.Settings.TileConnectionSettings.UsesSprites())
            {
                float xScale = tileConnectionXZSize / tileConnectionPrefabBoxSize.x;
                float yScale = tileConnectionXZSize / tileConnectionPrefabBoxSize.y;
                return new Vector3(tileConnectionPrefabTransform.lossyScale.x * xScale, tileConnectionPrefabTransform.lossyScale.y * yScale, 1.0f);
            }
            else
            {
                float xScale = tileConnectionXZSize / tileConnectionPrefabBoxSize.x;
                float zScale = tileConnectionXZSize / tileConnectionPrefabBoxSize.z;
                return new Vector3(tileConnectionPrefabTransform.lossyScale.x * xScale, tileConnectionPrefabTransform.lossyScale.y, tileConnectionPrefabTransform.lossyScale.z * zScale);
            }
        }
        #endregion
    }
}
#endif