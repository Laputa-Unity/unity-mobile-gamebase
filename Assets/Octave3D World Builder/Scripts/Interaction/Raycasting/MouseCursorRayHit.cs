#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public class MouseCursorRayHit
    {
        #region Private Variables
        private GridCellRayHit _gridCellRayHit;
        private List<GameObjectRayHit> _sortedObjectRayHits;

        private bool _wasParticleSystemHit = false;
        private bool _wasLightObjectHit = false;

        private GameObjectRayHit _closestLightObjectHit = null;
        private GameObjectRayHit _closestParticleSystemObjectHit = null;
        #endregion

        #region Public Properties
        public GridCellRayHit GridCellRayHit { get { return _gridCellRayHit; } }
        public List<GameObjectRayHit> SortedObjectRayHits { get { return _sortedObjectRayHits; } }
        public GameObjectRayHit ClosestObjectRayHit { get { return _sortedObjectRayHits.Count != 0 ? _sortedObjectRayHits[0] : null; } }
        
        public bool WasACellHit { get { return _gridCellRayHit != null; } }
        public bool WasAnObjectHit 
        { 
            get 
            {
                _sortedObjectRayHits.RemoveAll(item => item.HitObject == null);
                return _sortedObjectRayHits.Count != 0; 
            } 
        }

        public bool WasParticleSystemHit { get { return _wasParticleSystemHit; } }
        public bool WasLightObjectHit { get { return _wasLightObjectHit; } }
        public bool WasAnythingHit { get { return WasACellHit || WasAnObjectHit; } }
        #endregion

        #region Constructors
        public MouseCursorRayHit(GridCellRayHit gridCellRayHit, List<GameObjectRayHit> sortedObjectRayHits)
        {
            _gridCellRayHit = gridCellRayHit;
            _sortedObjectRayHits = sortedObjectRayHits != null ? new List<GameObjectRayHit>(sortedObjectRayHits) : new List<GameObjectRayHit>();
    
            int firstLightHit = _sortedObjectRayHits.FindIndex(item => item.HitObject.HasLight());
            int firstParticleSystemHit = _sortedObjectRayHits.FindIndex(item => item.HitObject.HasParticleSystem());

            if (firstLightHit >= 0)
            {
                _wasLightObjectHit = true;
                _closestLightObjectHit = _sortedObjectRayHits[firstLightHit];
            }
            if(firstParticleSystemHit >= 0)
            {
                _wasParticleSystemHit = true;
                _closestParticleSystemObjectHit = _sortedObjectRayHits[firstParticleSystemHit];
            }
        }
        #endregion

        #region Public Methods
        public GameObject GetClosestHitParticleSystemOrLightObject()
        {
            if(WasAnObjectHit && (WasLightObjectHit || WasParticleSystemHit))
            {
                GameObject closestHitObject = _closestLightObjectHit != null ? _closestLightObjectHit.HitObject : null;
                if (closestHitObject == null ||
                   (_closestParticleSystemObjectHit != null && _closestLightObjectHit.HitEnter > _closestParticleSystemObjectHit.HitEnter))
                    closestHitObject = _closestParticleSystemObjectHit.HitObject;

                return closestHitObject;
            }

            return null;
        }

        public List<GameObject> GetAllObjectsSortedByHitDistance()
        {
            if (!WasAnObjectHit) return new List<GameObject>();

            var allObjects = new List<GameObject>();
            foreach (var objectHit in _sortedObjectRayHits) allObjects.Add(objectHit.HitObject);

            return allObjects;
        }
        #endregion
    }
}
#endif