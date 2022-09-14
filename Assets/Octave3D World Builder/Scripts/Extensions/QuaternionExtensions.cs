#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class QuaternionExtensions
    {
        #region Public Static Functions
        public static Quaternion GetRelativeRotation(this Quaternion from, Quaternion to)
        {
            return to * Quaternion.Inverse(from);
        }

        public static float GetMagnitude(this Quaternion quaternion)
        {
            return Mathf.Sqrt(quaternion.x * quaternion.x + quaternion.y * quaternion.y + quaternion.z * quaternion.z + quaternion.w * quaternion.w);
        }

        public static Quaternion NormalizeEx(this Quaternion quaternion)
        {
            Quaternion normalizedQuat = quaternion;
            float invMagnitude = 1.0f / quaternion.GetMagnitude();

            normalizedQuat.x *= invMagnitude;
            normalizedQuat.y *= invMagnitude;
            normalizedQuat.z *= invMagnitude;
            normalizedQuat.w *= invMagnitude;

            return normalizedQuat;
        }
        #endregion
    }
}
#endif