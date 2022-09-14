#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class RayExtensions
    {
        #region Extension Methods
        public static Ray Transform(this Ray ray, Matrix4x4 transformMatrix)
        {
            ray.origin = transformMatrix.MultiplyPoint(ray.origin);
            ray.direction = transformMatrix.MultiplyVector(ray.direction);

            return ray;
        }

        public static Ray InverseTransform(this Ray ray, Matrix4x4 transformMatrix)
        {
            return ray.Transform(transformMatrix.inverse);
        }
        #endregion
    }
}
#endif