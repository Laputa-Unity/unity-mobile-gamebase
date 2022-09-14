#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class DecorPaintStrokeSurface
    {
        #region Private Variables
        [SerializeField]
        private bool _isValid;
        [SerializeField]
        private Vector3 _mouseCursorPickPoint;
        [SerializeField]
        private Vector3 _normal;

        [SerializeField]
        private DecorPaintStrokeSurfaceType _type;
        [SerializeField]
        private GameObject _surfaceObject;
        #endregion

        #region Public Properties
        public Vector3 MouseCursorPickPoint { get { return _mouseCursorPickPoint; } }
        public Vector3 Normal { get { return _normal; } }
        public Plane Plane { get { return new Plane(Normal, _mouseCursorPickPoint); } }
        public bool IsValid { get { return _isValid; } }
        public DecorPaintStrokeSurfaceType Type { get { return _type; } }
        public GameObject SurfaceObject { get { return _surfaceObject; } }
        #endregion

        #region Public Methods
        public void FromMouseCursorRayHit(MouseCursorRayHit cursorRayHit)
        {
            _isValid = false;

            if (!cursorRayHit.WasAnythingHit) return;

            if (cursorRayHit.WasAnObjectHit)
            {
                GameObjectRayHit objectRayHit = FindClosestHitObjectWhichCanBeUsedAsPaintSurface(cursorRayHit);
                if (objectRayHit != null && ValidateGameObjectRayHit(objectRayHit)) ExtractData(objectRayHit);
            }

            if (!_isValid && cursorRayHit.WasACellHit) ExtractData(cursorRayHit.GridCellRayHit);
        }

        public void FromGameObjectRayHit(GameObjectRayHit gameObjectRayHit)
        {
            _isValid = false;

            if (!ValidateGameObjectRayHit(gameObjectRayHit)) return;
            ExtractData(gameObjectRayHit);
        }

        public void FromGirdCellRayHit(GridCellRayHit gridCellRayHit)
        {
            _isValid = false;
            ExtractData(gridCellRayHit);
        }
        #endregion

        #region Private Methods
        private GameObjectRayHit FindClosestHitObjectWhichCanBeUsedAsPaintSurface(MouseCursorRayHit cursorRayHit)
        {
            ObjectMask decorPaintMask = DecorPaintObjectPlacement.Get().DecorPaintMask;

            List<GameObjectRayHit> gameObjectHits = cursorRayHit.SortedObjectRayHits;
            for (int hitIndex = 0; hitIndex < gameObjectHits.Count; ++hitIndex)
            {
                GameObject hitObject = gameObjectHits[hitIndex].HitObject;

                if (!hitObject.HasMesh() && !hitObject.HasTerrain() && !hitObject.IsSprite()) continue;
                if (decorPaintMask.IsGameObjectMasked(hitObject)) continue;

                return gameObjectHits[hitIndex];
            }

            return null;
        }

        private bool ValidateGameObjectRayHit(GameObjectRayHit gameObjectRayHit)
        {
            if (gameObjectRayHit.WasBoxHit) return false;
            return true;
        }

        private void ExtractData(GameObjectRayHit gameObjectRayHit)
        {
            _isValid = true;
            _mouseCursorPickPoint = gameObjectRayHit.HitPoint;
            _normal = gameObjectRayHit.HitNormal;

            if (gameObjectRayHit.WasTerrainHit) _type = DecorPaintStrokeSurfaceType.Terrain;
            else if (gameObjectRayHit.WasSpriteHit) _type = DecorPaintStrokeSurfaceType.Sprite;
            else _type = DecorPaintStrokeSurfaceType.Mesh;

            _surfaceObject = gameObjectRayHit.HitObject;
        }

        private void ExtractData(GridCellRayHit gridCellRayHit)
        {
            _isValid = true;
            _mouseCursorPickPoint = gridCellRayHit.HitPoint;
            _normal = gridCellRayHit.HitNormal;
            _type = DecorPaintStrokeSurfaceType.GridCell;
            _surfaceObject = null;
        }
        #endregion
    }
}
#endif