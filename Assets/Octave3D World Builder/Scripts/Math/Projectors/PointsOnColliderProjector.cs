#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class PointsOnColliderProjector
    {
        #region Private Variables
        private float _projectedPointOffset = 0.001f;

        private Octave3DCollider _collider;
        private Plane _planeOfPointsToProject = new Plane(Vector3.zero, Vector3.up);
        #endregion

        #region Public Properties
        public float ProjectedPointOffset { get { return _projectedPointOffset; } set { _projectedPointOffset = value; } }
        public Octave3DCollider Collider { get { return _collider; } set { if (value != null) _collider = value; } }
        public Plane PlaneOfPointsToProject { get { return _planeOfPointsToProject; } set { _planeOfPointsToProject = value; } }
        #endregion

        #region Constructors
        public PointsOnColliderProjector()
        {
        }

        public PointsOnColliderProjector(Octave3DCollider collider, Plane planeOfPointsToProject)
        {
            _collider = collider;
            _planeOfPointsToProject = planeOfPointsToProject;
        }
        #endregion

        #region Public Methods
        public List<Vector3> ProjectPoints(List<Vector3> pointsToProject)
        {
            if (!CanProjectPoints()) return new List<Vector3>();

            var projectedPoints = new List<Vector3>(pointsToProject.Count);
            for (int pointIndex = 0; pointIndex < pointsToProject.Count; ++pointIndex)
            {
                Vector3 projectedPoint;
                if (ProjectPoint(pointsToProject[pointIndex], out projectedPoint)) projectedPoints.Add(projectedPoint);
                else projectedPoints.Add(pointsToProject[pointIndex]);
            }

            return projectedPoints;
        }

        public bool ProjectPoint(Vector3 point, out Vector3 projectedPoint)
        {
            projectedPoint = Vector3.zero;
            if (!CanProjectPoints()) return false;

            if (ProjectPointByCastingRay(point, _planeOfPointsToProject.normal, out projectedPoint)) return true;
            return false;
        }

        public bool ProjectPoint(Vector3 point, out Vector3 projectedPoint, out Vector3 normalAtPointOfProjection)
        {
            projectedPoint = Vector3.zero;
            normalAtPointOfProjection = Vector3.zero;

            if (!CanProjectPoints()) return false;

            if (ProjectPointByCastingRay(point, _planeOfPointsToProject.normal, out projectedPoint, out normalAtPointOfProjection)) return true;
            return false;
        }
        #endregion

        #region Private Methods
        private bool CanProjectPoints()
        {
            return _collider != null;
        }

        private bool ProjectPointByCastingRay(Vector3 raycastOrigin, Vector3 raycastDirection, out Vector3 projectedPoint)
        {
            projectedPoint = Vector3.zero;
        
            Octave3DColliderRayHit colliderRayHit;
            Ray ray = new Ray(raycastOrigin, raycastDirection);
            if(_collider.RaycastBothDirections(ray, out colliderRayHit))
            {
                projectedPoint = colliderRayHit.HitPoint + _planeOfPointsToProject.normal * _projectedPointOffset;
                return true;
            }

            return false;
        }

        private bool ProjectPointByCastingRay(Vector3 raycastOrigin, Vector3 raycastDirection, out Vector3 projectedPoint, out Vector3 normalAtPointOfProjection)
        {
            projectedPoint = Vector3.zero;
            normalAtPointOfProjection = Vector3.zero;

            Octave3DColliderRayHit colliderRayHit;
            Ray ray = new Ray(raycastOrigin, raycastDirection);
            if (_collider.RaycastBothDirections(ray, out colliderRayHit))
            {
                projectedPoint = colliderRayHit.HitPoint + _planeOfPointsToProject.normal * _projectedPointOffset;
                normalAtPointOfProjection = colliderRayHit.HitNormal;
             
                return true;
            }

            return false;
        }
        #endregion
    }
}
#endif