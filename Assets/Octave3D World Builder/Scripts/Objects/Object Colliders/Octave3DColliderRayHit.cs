#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class Octave3DColliderRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        private Octave3DCollider _hitCollider;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        public Octave3DCollider HitCollider { get { return _hitCollider; } }
        #endregion

        #region Constructors
        public Octave3DColliderRayHit(Ray ray, float hitEnter, Vector3 hitPoint, Vector3 hitNormal, Octave3DCollider hitCollider)
        {
            _ray = ray;
            _hitEnter = hitEnter;
            _hitCollider = hitCollider;

            _hitPoint = hitPoint;
            _hitNormal = hitNormal;
            _hitNormal.Normalize();
        }
        #endregion
    }
}
#endif