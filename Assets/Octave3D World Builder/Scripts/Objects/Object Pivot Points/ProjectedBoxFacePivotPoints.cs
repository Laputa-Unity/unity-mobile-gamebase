#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class ProjectedBoxFacePivotPoints
    {
        #region Private Variables
        [SerializeField]
        private PivotPointCollection _pivotPointCollection = new PivotPointCollection();
        [SerializeField]
        private List<Vector3> _unprojectedPivotPoints = new List<Vector3> { Vector3.zero };
        [SerializeField]
        private Plane _pointsPlane = new Plane();

        [SerializeField]
        private float _area = 0.0f;
        #endregion

        #region Public Static Properties
        public static int NumberOfBoundarySegments { get { return 4; } }
        public static int IndexOfPointInCenter { get { return 0; } }
        #endregion

        #region Public Properties
        public int NumberOfPoints { get { return _pivotPointCollection.NumberOfPoints; } }
        public Vector3 ActivePoint { get { return _pivotPointCollection.ActivePoint; } }
        public Vector3 CenterPoint { get { return _pivotPointCollection.GetPointByIndex(IndexOfPointInCenter); } }
        public int IndexOfActivePoint { get { return _pivotPointCollection.IndexOfActivePoint; } }
        public List<Vector3> AllPoints { get { return _pivotPointCollection.AllPoints; } }
        public Plane PointsPlane { get { return _pointsPlane; } }
        public float Area { get { return _area; } }
        #endregion

        #region Public Methods
        public ProjectedBoxFacePivotPoints TakeSnapshot()
        {
            var projectedBoxFacePivotPoints = new ProjectedBoxFacePivotPoints();

            projectedBoxFacePivotPoints._pivotPointCollection = _pivotPointCollection.TakeSnapshot();
            projectedBoxFacePivotPoints._unprojectedPivotPoints = GetUnprojectedPivotPoints();
            projectedBoxFacePivotPoints._pointsPlane = PointsPlane;

            return projectedBoxFacePivotPoints;
        }

        public void FromOrientedBoxAndSnapSurface(OrientedBox orientedBox, SnapSurface objectSnapSurface)
        {
            if (!objectSnapSurface.IsValid) return;

            Vector3 surfacePlaneNormal = objectSnapSurface.Plane.normal;
            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) surfacePlaneNormal *= -1.0f;

            BoxFace boxFaceMostAlignedWithSurface = orientedBox.GetBoxFaceWhichFacesNormal(surfacePlaneNormal);
            List<Vector3> cornerPointsOfFaceMostAlignedWithSurface = orientedBox.GetBoxFaceCornerPoints(boxFaceMostAlignedWithSurface);

            CalculatePointsPlane(orientedBox, objectSnapSurface);

            StoreObjectPivotPoints(_pointsPlane.ProjectAllPoints(cornerPointsOfFaceMostAlignedWithSurface));
            StoreUnprojectedPivotPoints(cornerPointsOfFaceMostAlignedWithSurface);
            CalculateArea();
        }

        public void FromOrientedBoxAndDecorStrokeSurface(OrientedBox orientedBox, DecorPaintStrokeSurface decorPaintStrokeSurface)
        {
            if (!decorPaintStrokeSurface.IsValid) return;

            Vector3 surfacePlaneNormal = decorPaintStrokeSurface.Plane.normal;
            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) surfacePlaneNormal *= -1.0f;

            BoxFace boxFaceMostAlignedWithSurface = orientedBox.GetBoxFaceWhichFacesNormal(surfacePlaneNormal);
            List<Vector3> cornerPointsOfFaceMostAlignedWithSurface = orientedBox.GetBoxFaceCornerPoints(boxFaceMostAlignedWithSurface);

            CalculatePointsPlane(orientedBox, decorPaintStrokeSurface);

            StoreObjectPivotPoints(_pointsPlane.ProjectAllPoints(cornerPointsOfFaceMostAlignedWithSurface));
            StoreUnprojectedPivotPoints(cornerPointsOfFaceMostAlignedWithSurface);
            CalculateArea();
        }

        public List<Vector3> GetAllPointsExcludingCenter()
        {
            List<Vector3> pivotPoints = AllPoints;
            if (pivotPoints.Count == 0) return pivotPoints;

            pivotPoints.RemoveAt(IndexOfPointInCenter);

            return pivotPoints;
        }

        public int GetIndexOfPointClosestToPoint(Vector3 point)
        {
            return Vector3Extensions.GetIndexOfPointClosestToPoint(AllPoints, point);
        }

        public List<Vector3> GetUnprojectedPivotPoints()
        {
            return new List<Vector3>(_unprojectedPivotPoints);
        }

        public void ActivatePoint(int pointIndex)
        {
            _pivotPointCollection.ActivatePoint(pointIndex);
        }

        public void ActivateNextPivotPoint()
        {
            _pivotPointCollection.ActivateNextPoint();
        }

        public Vector3 GetPointByIndex(int pointIndex)
        {
            return _pivotPointCollection.GetPointByIndex(pointIndex);
        }

        public List<Segment3D> GetAllBoundarySegments()
        {
            List<Vector3> allPointsNoCenter = GetAllPointsExcludingCenter();
            return Vector3Extensions.GetSegmentsBetweenPoints(allPointsNoCenter, true);
        }

        public List<Plane> GetAllBoundarySegmentPlanes()
        {
            List<Segment3D> allBoundarySegments = GetAllBoundarySegments();
            var allBoundarySegmentPlanes = new List<Plane>(allBoundarySegments.Count);
            for(int segmentIndex = 0; segmentIndex < allBoundarySegments.Count; ++segmentIndex)
            {
                allBoundarySegmentPlanes.Add(GetBoundarySegmentPlane(segmentIndex));
            }

            return allBoundarySegmentPlanes;
        }

        public Segment3D GetBoundarySegment(int segmentIndex)
        {
            segmentIndex %= NumberOfBoundarySegments;
            List<Vector3> allPointsNoCenter = GetAllPointsExcludingCenter();

            return new Segment3D(allPointsNoCenter[segmentIndex], allPointsNoCenter[(segmentIndex + 1) % NumberOfBoundarySegments]);
        }

        public Plane GetBoundarySegmentPlane(int segmentIndex)
        {
            Segment3D boundarySegment = GetBoundarySegment(segmentIndex);

            Vector3 centerPivotPoint = GetPointByIndex(IndexOfPointInCenter);
            Vector3 centerPivotPointProjectedOnSegment = centerPivotPoint.CalculateProjectionPointOnSegment(boundarySegment.StartPoint, boundarySegment.EndPoint);
            Vector3 planeNormal = centerPivotPointProjectedOnSegment - centerPivotPoint;
            planeNormal.Normalize();

            return new Plane(planeNormal, boundarySegment.StartPoint);
        }

        public void MovePoints(Vector3 moveVector)
        {
            _unprojectedPivotPoints = Vector3Extensions.ApplyOffsetToPoints(_unprojectedPivotPoints, moveVector);
            _pivotPointCollection.MovePoints(moveVector);
            _pointsPlane = new Plane(_pointsPlane.normal, _pivotPointCollection.GetPointByIndex(0));
        }
        #endregion

        #region Private Methods
        private void StoreUnprojectedPivotPoints(List<Vector3> cornerPointsOfFaceMostAlignedWithSurface)
        {
            _unprojectedPivotPoints = new List<Vector3>(cornerPointsOfFaceMostAlignedWithSurface);
            _unprojectedPivotPoints.Insert(IndexOfPointInCenter, Vector3Extensions.GetAveragePoint(cornerPointsOfFaceMostAlignedWithSurface));
        }

        private void CalculatePointsPlane(OrientedBox orientedBox, SnapSurface objectSnapSurface)
        {
            // Note: It is necessary to ensure that all projected points sit right below the oriented box in order
            //       to avoid situations in which the object hierarchy floats in the air or becomes embedded
            //       inside other objects. In order to do this, we adjust the surface plane such that the box
            //       sits right in front of it (or behind) depending on the settings.
            _pointsPlane = objectSnapSurface.Plane;
            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) _pointsPlane = _pointsPlane.AdjustSoBoxSitsBehind(orientedBox);
            else _pointsPlane = _pointsPlane.AdjustSoBoxSitsInFront(orientedBox);
        }

        private void CalculatePointsPlane(OrientedBox orientedBox, DecorPaintStrokeSurface decorPaintSurface)
        {
            _pointsPlane = decorPaintSurface.Plane;
            if (AllShortcutCombos.Instance.PlaceGuideBehindSurfacePlane.IsActive()) _pointsPlane = _pointsPlane.AdjustSoBoxSitsBehind(orientedBox);
            else _pointsPlane = _pointsPlane.AdjustSoBoxSitsInFront(orientedBox);
        }

        private void StoreObjectPivotPoints(List<Vector3> projectedFacePoints)
        {
            List<Vector3> projectedPointsAndCenter = new List<Vector3>(projectedFacePoints);
            projectedPointsAndCenter.Insert(IndexOfPointInCenter, Vector3Extensions.GetAveragePoint(projectedFacePoints));

            _pivotPointCollection.SetPivotPoints(projectedPointsAndCenter, _pivotPointCollection.IndexOfActivePoint);
        }

        private void CalculateArea()
        {
            List<Segment3D> boundarySegments = GetAllBoundarySegments();
            for(int segmentIndex = 0; segmentIndex < boundarySegments.Count; ++segmentIndex)
            {
                Segment3D currentSegment = boundarySegments[segmentIndex];
                for(int perpSegmentIndex = segmentIndex + 1; perpSegmentIndex < boundarySegments.Count; ++perpSegmentIndex)
                {
                    Segment3D perpSegment  = boundarySegments[perpSegmentIndex];
                    if(perpSegment.IsPerpendicualrTo(currentSegment))
                    {
                        Vector3 cross = Vector3.Cross(perpSegment.Direction, currentSegment.Direction);
                        _area = cross.magnitude;
                        return;
                    }
                }
            }
        }
        #endregion
    }
}
#endif