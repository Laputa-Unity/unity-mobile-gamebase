#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class Octave3DTerrainCollider : Octave3DCollider
    {
        #region Private Methods
        private TerrainCollider _terrainCollider;
        #endregion

        #region Public Properties
        public TerrainCollider TerrainCollider { get { return _terrainCollider; } }
        #endregion

        #region Constructors
        public Octave3DTerrainCollider(TerrainCollider terrainCollider)
        {
            _terrainCollider = terrainCollider;
        }
        #endregion

        #region Public Methods
        public override Octave3DColliderType GetColliderType()
        {
            return Octave3DColliderType.Terrain;
        }

        public override bool Raycast(Ray ray, out Octave3DColliderRayHit colliderRayHit)
        {
            colliderRayHit = null;

            RaycastHit rayHit;
            if (_terrainCollider.Raycast(ray, out rayHit, float.MaxValue)) 
                colliderRayHit = new Octave3DColliderRayHit(ray, rayHit.distance, rayHit.point, rayHit.normal, this);

            return colliderRayHit != null;
        }
        #endregion
    }
}
#endif