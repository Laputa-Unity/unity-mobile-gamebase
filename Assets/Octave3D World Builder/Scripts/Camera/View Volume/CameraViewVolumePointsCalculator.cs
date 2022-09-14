#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public abstract class CameraViewVolumePointsCalculator
    {
        #region Public Abstract Methods
        public abstract Vector3[] CalculateWorldSpaceVolumePoints(Camera camera);
        #endregion
    }
}
#endif