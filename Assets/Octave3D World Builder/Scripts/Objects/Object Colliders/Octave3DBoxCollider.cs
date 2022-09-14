#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class Octave3DBoxCollider : Octave3DCollider
    {
        #region Private Variables
        private OrientedBox _orientedBox;
        #endregion

        #region Public Properties
        public OrientedBox OrientedBox { get { return new OrientedBox(_orientedBox); } }
        public Box ModelSpaceBox { get { return _orientedBox.ModelSpaceBox; } }
        #endregion

        #region Constructors
        public Octave3DBoxCollider(OrientedBox orientedBox)
        {
            _orientedBox = new OrientedBox(orientedBox);
        }
        #endregion

        #region Public Methods
        public override Octave3DColliderType GetColliderType()
        {
            return Octave3DColliderType.Box;
        }

        public override bool Raycast(Ray ray, out Octave3DColliderRayHit colliderRayHit)
        {
            colliderRayHit = null;
            float t;

            if (_orientedBox.Raycast(ray, out t))
            {
                Vector3 hitPoint = ray.GetPoint(t);
                BoxFace faceWhichContainsHitPoint = _orientedBox.GetBoxFaceClosestToPoint(hitPoint);
                Vector3 hitNormal = _orientedBox.GetBoxFacePlane(faceWhichContainsHitPoint).normal;

                colliderRayHit = new Octave3DColliderRayHit(ray, t, hitPoint, hitNormal, this);
            }
            return colliderRayHit != null;
        }
        #endregion
    }
}
#endif