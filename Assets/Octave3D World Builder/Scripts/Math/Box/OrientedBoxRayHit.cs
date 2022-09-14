#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class OrientedBoxRayHit
    {
        #region Private Variables
        private Ray _ray;
        private float _hitEnter;
        private OrientedBox _hitBox;
        private Octave3DBoxCollider _hitCollider;
        private Vector3 _hitPoint;
        private Vector3 _hitNormal;
        private BoxFace _hitFace;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public float HitEnter { get { return _hitEnter; } }
        public OrientedBox HitBox { get { return new OrientedBox(_hitBox); } }
        public Octave3DBoxCollider HitCollider { get { return _hitCollider; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        public Vector3 HitNormal { get { return _hitNormal; } }
        public BoxFace HitFace { get { return _hitFace; } }
        #endregion

        #region Constructors
        public OrientedBoxRayHit(Ray ray, float hitEnter, OrientedBox hitBox)
        {
            _ray = ray;
            _hitEnter = hitEnter;
            _hitBox = new OrientedBox(hitBox);
            _hitCollider = new Octave3DBoxCollider(_hitBox);
            _hitPoint = ray.GetPoint(hitEnter);

            _hitFace = hitBox.GetBoxFaceClosestToPoint(_hitPoint);
            _hitNormal = hitBox.GetBoxFacePlane(_hitFace).normal;

            CorrectHitDataForThinPlanes();
        }
        #endregion

        #region Private Methods
        private void CorrectHitDataForThinPlanes()
        {
            float boxSizeAlongHitNormal = _hitBox.GetSizeAlongDirection(_hitNormal);
            if(boxSizeAlongHitNormal < 1e-4f && 
               Vector3.Dot(_hitNormal, SceneViewCamera.Camera.transform.forward) > 0.0f)
            {
                _hitFace = BoxFaces.GetOpposite(_hitFace);
                _hitNormal = _hitBox.GetBoxFacePlane(_hitFace).normal;
            }
        }
        #endregion
    }
}
#endif