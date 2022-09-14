#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class MeshRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private Octave3DMesh _hitMesh;
        private Octave3DMeshCollider _hitCollider;
        private int _hitTraingleIndex;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public Octave3DMesh HitMesh { get { return _hitMesh; } }
        public Octave3DMeshCollider HitCollider { get { return _hitCollider; } }
        public int HitTriangleIndex { get { return _hitTraingleIndex; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        #endregion

        #region Constructors
        public MeshRayHit(Ray ray, float hitEnter, Octave3DMesh hitMesh, int hitTriangleIndex, Vector3 hitPoint, Vector3 hitNormal, TransformMatrix meshTransformMatrix)
        {
            _ray = ray;
            _hitEnter = hitEnter;
            _hitMesh = hitMesh;
            _hitCollider = new Octave3DMeshCollider(_hitMesh, meshTransformMatrix);
            _hitTraingleIndex = hitTriangleIndex;
            _hitPoint = hitPoint;

            _hitNormal = hitNormal;
            _hitNormal.Normalize();
        }
        #endregion
    }
}
#endif