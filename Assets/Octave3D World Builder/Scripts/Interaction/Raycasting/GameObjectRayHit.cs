#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public class GameObjectRayHit
    {
        #region Private Variables
        private Ray _ray;
        private GameObject _hitObject;
        private OrientedBoxRayHit _objectBoxHit;
        private MeshRayHit _objectMeshHit;
        private TerrainRayHit _objectTerrainHit;
        private SpriteRayHit _objectSpriteHit;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public GameObject HitObject { get { return _hitObject; } }
        public OrientedBoxRayHit ObjectBoxHit { get { return _objectBoxHit; } }
        public MeshRayHit ObjectMeshHit { get { return _objectMeshHit; } }
        public TerrainRayHit ObjectTerrainHit { get { return _objectTerrainHit; } }
        public SpriteRayHit ObjectSpriteHit { get { return _objectSpriteHit; } }

        public bool WasBoxHit { get { return _objectBoxHit != null; } }
        public bool WasMeshHit { get { return _objectMeshHit != null && _hitObject != null; } }
        public bool WasTerrainHit { get { return _objectTerrainHit != null && _hitObject != null; } }
        public bool WasSpriteHit { get { return _objectSpriteHit != null && _hitObject != null; } }

        public Vector3 HitPoint
        {
            get
            {
                if (WasBoxHit) return _objectBoxHit.HitPoint;
                if (WasMeshHit) return _objectMeshHit.HitPoint;
                if (WasTerrainHit) return _objectTerrainHit.HitPoint;
                if (WasSpriteHit) return _objectSpriteHit.HitPoint;

                return Vector3.zero;
            }
        }

        public Vector3 HitNormal
        {
            get
            {
                if (WasBoxHit) return _objectBoxHit.HitNormal;
                if (WasMeshHit) return _objectMeshHit.HitNormal;
                if (WasTerrainHit) return _objectTerrainHit.HitNormal;
                if (WasSpriteHit) return _objectSpriteHit.HitNormal;

                return Vector3.zero;
            }
        }

        public Octave3DCollider HitCollider
        {
            get
            {
                if (WasBoxHit) return _objectBoxHit.HitCollider;
                if (WasMeshHit) return _objectMeshHit.HitCollider;
                if (WasTerrainHit) return _objectTerrainHit.HitCollider;
                if (WasSpriteHit) return _objectSpriteHit.HitCollider;

                return null;
            }
        }

        public float HitEnter
        {
            get
            {
                if (WasBoxHit) return _objectBoxHit.HitEnter;
                if (WasMeshHit) return _objectMeshHit.HitEnter;
                if (WasTerrainHit) return _objectTerrainHit.HitEnter;
                if (WasSpriteHit) return _objectSpriteHit.HitEnter;

                return 0.0f;
            }
        }
        #endregion

        #region Constructors
        public GameObjectRayHit(Ray ray, GameObject hitObject, OrientedBoxRayHit objectBoxHit, MeshRayHit objectMeshHit, TerrainRayHit objectTerrainHit, SpriteRayHit objectSpriteHit)
        {
            _ray = ray;
            _hitObject = hitObject;

            // Only one kind of entity can be registered as a hit, so we will take the first
            // non-null hit instance using the following priority: terrain, mesh, sprite, box.
            if(objectTerrainHit != null)
            {
                _objectTerrainHit = objectTerrainHit;
                _objectBoxHit = null;
                _objectMeshHit = null;
                _objectSpriteHit = null;
            }
            else
            if (objectMeshHit != null)
            {
                _objectTerrainHit = null;
                _objectBoxHit = null;
                _objectMeshHit = objectMeshHit;
                _objectSpriteHit = null;
            }
            else
            if (objectSpriteHit != null)
            {
                _objectTerrainHit = null;
                _objectBoxHit = null;
                _objectMeshHit = null;
                _objectSpriteHit = objectSpriteHit;
            }
            if(objectBoxHit != null)
            {
                _objectTerrainHit = null;
                _objectBoxHit = objectBoxHit;
                _objectMeshHit = null;
                _objectSpriteHit = null;
            }
        }
        #endregion
    }
}
#endif