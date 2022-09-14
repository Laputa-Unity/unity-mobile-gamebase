#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class TerrainRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private Octave3DTerrainCollider _hitCollider;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public Octave3DTerrainCollider HitCollider { get { return _hitCollider; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        #endregion

        #region Constructors
        public TerrainRayHit(Ray ray, RaycastHit raycastHit)
        {
            _ray = ray;
            _hitEnter = raycastHit.distance;

            _hitCollider = new Octave3DTerrainCollider(raycastHit.collider as TerrainCollider);
            _hitPoint = raycastHit.point;
            _hitNormal = raycastHit.normal;
        }
        #endregion
    }
}
#endif