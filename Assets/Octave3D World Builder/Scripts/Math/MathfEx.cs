#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class MathfEx
    {
        #region Pulbic Static Functions
        public static float Max(float v1, float v2, float v3)
        {
            return Mathf.Max(Mathf.Max(v1, v2), v3);
        }

        public static float Min(float v1, float v2, float v3)
        {
            return Mathf.Min(Mathf.Min(v1, v2), v3);
        }
        #endregion
    }
}
#endif