#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class SpriteRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private SpriteRenderer _hitSpriteRenderer;
        private Octave3DBoxCollider _hitCollider;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public Octave3DBoxCollider HitCollider { get { return _hitCollider; } }
        public SpriteRenderer HitSpriteRenderer {get{return _hitSpriteRenderer;}}
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        #endregion

        #region Constructors
        public SpriteRayHit(Ray ray, float hitEnter, SpriteRenderer hitSpriteRenderer, Vector3 hitPoint, Vector3 hitNormal)
        {
            _ray = ray;
            _hitEnter = hitEnter;
            _hitSpriteRenderer = hitSpriteRenderer;
            _hitCollider = new Octave3DBoxCollider(hitSpriteRenderer.gameObject.GetWorldOrientedBox());
            _hitPoint = hitPoint;

            _hitNormal = hitNormal;
            _hitNormal.Normalize();
        }
        #endregion
    }
}
#endif