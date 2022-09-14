#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class Octave3DMeshCollider : Octave3DCollider
    {
        #region Private Variables
        private Octave3DMesh _mesh;
        private TransformMatrix _meshTransform;
        #endregion

        #region Public Properties
        public Octave3DMesh Mesh { get { return _mesh; } }
        public TransformMatrix MeshTransform { get { return _meshTransform; } }
        #endregion

        #region Constructors
        public Octave3DMeshCollider(Octave3DMesh mesh, TransformMatrix meshTransform)
        {
            _mesh = mesh;
            _meshTransform = meshTransform;
        }
        #endregion

        #region Public Methods
        public override Octave3DColliderType GetColliderType()
        {
            return Octave3DColliderType.Mesh;
        }

        public override bool Raycast(Ray ray, out Octave3DColliderRayHit colliderRayHit)
        {
            colliderRayHit = null;
            MeshRayHit meshRayHit = Mesh.Raycast(ray, _meshTransform);
            if (meshRayHit != null) colliderRayHit = new Octave3DColliderRayHit(ray, meshRayHit.HitEnter, meshRayHit.HitPoint, meshRayHit.HitNormal, this);

            return colliderRayHit != null;
        }
        #endregion
    }
}
#endif