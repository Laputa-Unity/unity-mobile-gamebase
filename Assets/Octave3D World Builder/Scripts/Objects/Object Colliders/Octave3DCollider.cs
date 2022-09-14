#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public abstract class Octave3DCollider
    {
        #region Public Methods
        public bool Raycast(Ray ray)
        {
            Octave3DColliderRayHit colliderRayHit;
            return Raycast(ray, out colliderRayHit);
        }

        public bool RaycastBothDirections(Ray ray, out Octave3DColliderRayHit colliderRayHit)
        {
            const float originOffsetAlongReverseDirection = 0.001f;
            Ray offsetRay = ray;
            offsetRay.origin -= offsetRay.direction * originOffsetAlongReverseDirection;

            if (Raycast(offsetRay, out colliderRayHit)) return true;
            else
            {
                offsetRay.direction = -offsetRay.direction;
                offsetRay.origin = ray.origin - offsetRay.direction * originOffsetAlongReverseDirection;
                if (Raycast(offsetRay, out colliderRayHit)) return true;
            }

            return false;
        }
        #endregion

        #region Public Abstract Methods
        public abstract Octave3DColliderType GetColliderType();
        public abstract bool Raycast(Ray ray, out Octave3DColliderRayHit colliderRayHit);
        #endregion
    }
}
#endif