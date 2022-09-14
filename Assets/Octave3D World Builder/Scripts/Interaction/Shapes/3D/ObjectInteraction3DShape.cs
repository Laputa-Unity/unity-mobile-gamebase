#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public abstract class ObjectInteraction3DShape
    {
        #region Private Constant Variables
        private const float _allowedObjectBoxDistanceFromSurfacePlaneForOverlapDetection = 0.12f;
        #endregion

        #region Protected Variables
        protected List<Vector3> _renderPoints = new List<Vector3>();
        protected bool _renderPointsMustBeCalculated = true;

        protected Vector3 _furthestRenderPointInFrontOfShapePlane;
        protected Vector3 _furthestRenderPointBehindShapePlane;

        protected MouseCursorRayHit _cursorRayHit;
        #endregion

        #region Public Properties
        public Plane Plane { get { return new Plane(Normal, Center); } }
        public Vector3 FurthestRenderPointInFrontOfShapePlane 
        { 
            get 
            {
                if (_renderPointsMustBeCalculated) CalculateRenderPoints();
                return _furthestRenderPointInFrontOfShapePlane; 
            } 
        }
        public Vector3 FurthestRenderPointBehindShapePlane 
        { 
            get 
            {
                if (_renderPointsMustBeCalculated) CalculateRenderPoints();
                return _furthestRenderPointBehindShapePlane; 
            } 
        }
        public MouseCursorRayHit CursorRayHit { get { return _cursorRayHit; } }
        #endregion

        #region Public Abstract Properties
        public abstract Vector3 Center { get; set; }
        public abstract Vector3 Normal { get; set; }
        #endregion

        #region Public Methods
        public List<Vector3> GetRenderPoints()
        {
            if (_renderPointsMustBeCalculated) CalculateRenderPoints();
            return new List<Vector3>(_renderPoints);
        }

        public void AcquireCursorRayHit(ObjectMask objectMask, MouseCursorObjectPickFlags cursorPickMaskFlags)
        {
            MouseCursor.Instance.PushObjectMask(objectMask);
            MouseCursor.Instance.PushObjectPickMaskFlags(cursorPickMaskFlags);

            _cursorRayHit = MouseCursor.Instance.GetRayHit();

            MouseCursor.Instance.PopObjectMask();
            MouseCursor.Instance.PopObjectPickMaskFlags();
        }

        public void AcquireCursorRayHitForTerrainObject(GameObject terrainObject)
        {
            _cursorRayHit = MouseCursor.Instance.GetCursorRayHitForTerrainObject(terrainObject);
        }

        public void AdjustShapeCenterAndNormalBasedOnCursorRayHit()
        {
            if (HasCursorPickedObject())
            {
                GameObjectRayHit objectRayHit = _cursorRayHit.ClosestObjectRayHit;

                if (objectRayHit.WasTerrainHit) AdjustShapeWhenSittingOnTerrain(objectRayHit);
                else if (objectRayHit.WasMeshHit) AdjustShapeWhenSittingOnMesh(objectRayHit);
                else if (objectRayHit.WasSpriteHit) AdjustShapeWhenSittingOnSprite(objectRayHit);
            }
            else
            if (HasCursorPickedGridCell()) AdjustShapeWhenSittingOnGridCell();
        }

        public List<GameObject> GetPotentialyOverlappedGameObjects()
        {
            Sphere overlapSphere = new Sphere(Center, CalculateObjectOverlapSphereRadius());
            return Octave3DScene.Get().OverlapSphere(overlapSphere);
        }

        public List<GameObject> GetOverlappedGameObjects(bool allowPartialOverlap)
        {
            if (!HasCursorPickedAnything()) return new List<GameObject>();
   
            List<GameObject> potentialyOverlappedObjects = GetPotentialyOverlappedGameObjects();
            if (allowPartialOverlap) return GetOverlappedGameObjectsForPartialOverlap(potentialyOverlappedObjects);
            else return GetOverlappedGameObjectsForFullOverlap(potentialyOverlappedObjects);
        }

        public bool HasCursorPickedAnything()
        {
            return IsCursorRayHitAvailable() && _cursorRayHit.WasAnythingHit;
        }

        public bool HasCursorPickedSprite()
        {
            return HasCursorPickedObject() && _cursorRayHit.ClosestObjectRayHit.WasSpriteHit;
        }

        public bool HasCursorPickedGridCell()
        {
            return IsCursorRayHitAvailable() && _cursorRayHit.WasACellHit;
        }

        public bool HasCursorPickedObject()
        {
            return IsCursorRayHitAvailable() && _cursorRayHit.WasAnObjectHit;
        }

        public bool HasCursorPickedTerrainObject()
        {
            return HasCursorPickedObject() && _cursorRayHit.ClosestObjectRayHit.WasTerrainHit;
        }

        public bool HasCursorPickedMeshObject()
        {
            return HasCursorPickedObject() && _cursorRayHit.ClosestObjectRayHit.WasMeshHit;
        }

        public bool IsCursorRayHitAvailable()
        {
            return _cursorRayHit != null;
        }

        public GameObject GetGameObjectPickedByCursor()
        {
            if (HasCursorPickedObject()) return _cursorRayHit.ClosestObjectRayHit.HitObject;
            return null;
        }
        #endregion

        #region Public Abstract Methods
        public abstract void RenderGizmos();
        public abstract bool OverlapsPolygon(Polygon3D polygon);
        public abstract bool ContainsPoint(Vector3 point);
        public abstract bool ContainsAllPoints(List<Vector3> points);
        public abstract bool ContainsAnyPoint(List<Vector3> points);
        #endregion

        #region Protected Methods
        protected void CalculateRenderPoints()
        {
            if (_renderPointsMustBeCalculated)
            {
                _renderPoints = GenerateRenderPoints();
                _renderPointsMustBeCalculated = false;

                if (HasCursorPickedObject()) ProjectRenderPointsOnHoveredObject();
                CalculateRenderPointsFurthestFromShapePlane();
            }
        }
        #endregion

        #region Protected Methods
        protected abstract List<Vector3> GenerateRenderPoints();
        #endregion

        #region Private Methods
        private void CalculateRenderPointsFurthestFromShapePlane()
        {
            if (!Plane.GetFurthestPointInFront(_renderPoints, out _furthestRenderPointInFrontOfShapePlane)) _furthestRenderPointInFrontOfShapePlane = _renderPoints[0];
            if (!Plane.GetFurthestPointBehind(_renderPoints, out _furthestRenderPointBehindShapePlane)) _furthestRenderPointBehindShapePlane = _renderPoints[0];
        }

        private float CalculateObjectOverlapSphereRadius()
        {
            float distanceFromCenterToFurthestPointInFrontOfPlane = (Center - FurthestRenderPointInFrontOfShapePlane).magnitude;
            float distanceFromCenterToFurthestPointBehindPlane = (Center - FurthestRenderPointBehindShapePlane).magnitude;

            float sphereRadius = distanceFromCenterToFurthestPointInFrontOfPlane;
            if (sphereRadius < distanceFromCenterToFurthestPointBehindPlane) sphereRadius = distanceFromCenterToFurthestPointBehindPlane;
        
            return sphereRadius;
        }

        private void AdjustShapeWhenSittingOnTerrain(GameObjectRayHit objectRayHit)
        {
            Normal = Vector3.up;
            Center = objectRayHit.HitPoint;
        }

        private void AdjustShapeWhenSittingOnMesh(GameObjectRayHit objectRayHit)
        {
            Normal = objectRayHit.HitNormal;
            Center = objectRayHit.HitPoint;
        }

        private void AdjustShapeWhenSittingOnSprite(GameObjectRayHit objectRayHit)
        {
            Normal = objectRayHit.HitNormal;
            Center = objectRayHit.HitPoint;
        }

        private void AdjustShapeWhenSittingOnGridCell()
        {
            Normal = _cursorRayHit.GridCellRayHit.HitCell.ParentGrid.TransformMatrix.GetNormalizedUpAxis();
            Center = _cursorRayHit.GridCellRayHit.HitPoint;
        }

        private List<GameObject> GetOverlappedGameObjectsForPartialOverlap(List<GameObject> potentialyOverlappedObjects)
        {
            var overlappedGameObjects = new List<GameObject>(potentialyOverlappedObjects.Count);
            foreach (GameObject gameObject in potentialyOverlappedObjects)
            {
                if (ShouldGameObjectBeIgnoredForOverlapDetection(gameObject)) continue;
                if (IsGameObjectPartiallyOverlapped(gameObject)) overlappedGameObjects.Add(gameObject);
            }

            return overlappedGameObjects;
        }

        private bool ShouldGameObjectBeIgnoredForOverlapDetection(GameObject gameObject)
        {
            if (gameObject == GetGameObjectPickedByCursor()) return true;
            if (ObjectQueries.IsGameObjectEmpty(gameObject)) return true;

            return false;
        }

        private bool IsGameObjectPartiallyOverlapped(GameObject gameObject)
        {
            OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
            if (worldOrientedBox.IsValid())
            {
                Polygon3D projectedBoxCornerPointsPoly = worldOrientedBox.Get3DPolygonFromCornerPointsProjectedOnPlane(Plane);
                if (OverlapsPolygon(projectedBoxCornerPointsPoly) &&
                    DoesWorldOrientedBoxIntersectOrResideOnShapeSurface(worldOrientedBox)) return true;
            }

            return false;
        }

        private bool DoesWorldOrientedBoxIntersectOrResideOnShapeSurface(OrientedBox worldOrientedBox)
        {
            return worldOrientedBox.IsSpanningOrIsCloseInFrontOrOnAnyPlane(GetPlanesOnWhichOrientedBoxResides(worldOrientedBox), _allowedObjectBoxDistanceFromSurfacePlaneForOverlapDetection);
        }

        private List<Plane> GetPlanesOnWhichOrientedBoxResides(OrientedBox worldOrientedBox)
        {
            if (HasCursorPickedTerrainObject())
            {
                List<Vector3> boxCenterAndCornerPoints = worldOrientedBox.GetCenterAndCornerPoints();
                var planes = new List<Plane>(boxCenterAndCornerPoints.Count);

                Octave3DColliderRayHit colliderRayHit;
                Octave3DCollider hitCollider = _cursorRayHit.ClosestObjectRayHit.HitCollider;
                foreach (Vector3 boxPoint in boxCenterAndCornerPoints)
                {
                    Ray ray = new Ray(boxPoint, Normal);
                    if (hitCollider.RaycastBothDirections(ray, out colliderRayHit)) planes.Add(new Plane(colliderRayHit.HitNormal, colliderRayHit.HitPoint));
                    else planes.Add(Plane);
                }

                return planes;
            }

            return new List<Plane> { Plane };
        }

        private List<GameObject> GetOverlappedGameObjectsForFullOverlap(List<GameObject> potentialyOverlappedObjects)
        {
            var overlappedGameObjects = new List<GameObject>(potentialyOverlappedObjects.Count);
            foreach (GameObject gameObject in potentialyOverlappedObjects)
            {
                if (ShouldGameObjectBeIgnoredForOverlapDetection(gameObject)) continue;
                if (IsGameObjectFullyOverlappedByCircle(gameObject)) overlappedGameObjects.Add(gameObject);
            }

            return overlappedGameObjects;
        }

        private bool IsGameObjectFullyOverlappedByCircle(GameObject gameObject)
        {
            OrientedBox worldOrientedBox = gameObject.GetWorldOrientedBox();
            return IsWorldOrientedBoxFullyOverlappedByCircle(worldOrientedBox);
        }

        private bool IsWorldOrientedBoxFullyOverlappedByCircle(OrientedBox worldOrientedBox)
        {
            if (worldOrientedBox.IsValid())
            {
                if (ContainsAllPoints(worldOrientedBox.GetCornerPointsProjectedOnPlane(Plane)) &&
                    DoesWorldOrientedBoxIntersectOrResideOnShapeSurface(worldOrientedBox)) return true;
            }

            return false;
        }

        private void ProjectRenderPointsOnHoveredObject()
        {
            if(HasCursorPickedMeshObject() || HasCursorPickedTerrainObject())
            {
                var pointsProjector = new PointsOnColliderProjector(_cursorRayHit.ClosestObjectRayHit.HitCollider, Plane);
                _renderPoints = pointsProjector.ProjectPoints(_renderPoints);
            }
        }
        #endregion
    }
}
#endif