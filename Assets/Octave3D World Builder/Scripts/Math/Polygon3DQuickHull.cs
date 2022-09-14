#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Polygon3DQuickHull
    {
        #region Private Variables
        private List<Vector3> _polygonPointsOnSamePlane;
        private Vector3 _polygonNormal;
        #endregion

        #region Constructors
        public Polygon3DQuickHull(List<Vector3> polygonPointsOnSamePlane, Vector3 polygonPlaneNormal)
        {
            _polygonPointsOnSamePlane = new List<Vector3>(polygonPointsOnSamePlane);
            _polygonNormal = polygonPlaneNormal;
            _polygonNormal.Normalize();
        }
        #endregion

        #region Public Methods
        public List<Segment3D> Calculate()
        {
            Vector3Extensions.EliminateDuplicatePoints(_polygonPointsOnSamePlane);
            if (_polygonPointsOnSamePlane.Count < 3) return new List<Segment3D>();

            List<Vector3> workingPointList = new List<Vector3>(_polygonPointsOnSamePlane);

            Vector3 minPoint, maxPoint;
            CalculateInitialMinMaxPoints(out minPoint, out maxPoint);
            workingPointList.RemoveAll(item => item == minPoint || item == maxPoint);
           
            List<Segment3D> allHullSegments = new List<Segment3D>();
            Segment3D minMaxSegment = new Segment3D(minPoint, maxPoint);
            Plane minMaxSegmentPlane = GetSegmentPlane(minMaxSegment);
            bool furthestPointIsBehind = false;
            int indexOfPointFurthestFromPlane = minMaxSegmentPlane.GetIndexOfFurthestPointInFront(workingPointList);
            if (indexOfPointFurthestFromPlane < 0)
            {
                indexOfPointFurthestFromPlane = minMaxSegmentPlane.GetIndexOfFurthestPointBehind(workingPointList);
                furthestPointIsBehind = true;
            }
            if (indexOfPointFurthestFromPlane >= 0)
            {
                Vector3 furthestPointFromPlane = workingPointList[indexOfPointFurthestFromPlane];
                workingPointList.RemoveAt(indexOfPointFurthestFromPlane);

                List<Segment3D> hullSegments = new List<Segment3D>();
                if(furthestPointIsBehind)
                {
                    hullSegments.Add(new Segment3D(minMaxSegment.EndPoint, furthestPointFromPlane));
                    hullSegments.Add(new Segment3D(furthestPointFromPlane, minMaxSegment.StartPoint));
                    hullSegments.Add(minMaxSegment);   
                    QuickHull(workingPointList, hullSegments);
                }
                else
                {
                    hullSegments.Add(new Segment3D(furthestPointFromPlane, minMaxSegment.EndPoint));
                    hullSegments.Add(new Segment3D(minMaxSegment.StartPoint, furthestPointFromPlane));
                    hullSegments.Add(new Segment3D(minMaxSegment.EndPoint, minMaxSegment.StartPoint));      // Reversed min-max segment
                    QuickHull(workingPointList, hullSegments);
                }
              
                allHullSegments.AddRange(hullSegments);
            }
            
            return allHullSegments;
        }
        #endregion

        #region Private Methods
        private void CalculateInitialMinMaxPoints(out Vector3 minPoint, out Vector3 maxPoint)
        {
            // We will sort the points along the X axis, but we have to take the polygon's coordinate
            // system into account because if we use the global X axis, we will run into trouble when
            // all points of the polygon reside on the YZ plane (i.e. same X coordinate).
            Vector3 polyLocalRight, polyLocalLook;
            if(_polygonNormal.IsAlignedWith(Vector3.up))
            {
                polyLocalRight = Vector3.right;
                polyLocalLook = Vector3.Cross(polyLocalRight, Vector3.up);
                polyLocalLook.Normalize();
            }
            else
            {
                polyLocalRight = Vector3.Cross(_polygonNormal, Vector3.up);
                polyLocalRight.Normalize();
                polyLocalLook = Vector3.Cross(polyLocalRight, _polygonNormal);
                polyLocalLook.Normalize();
            }

            Quaternion polyRotation = Quaternion.LookRotation(polyLocalLook, _polygonNormal);
            TransformMatrix transformMatrix = new TransformMatrix(Vector3.zero, polyRotation, Vector3.one);

            // Note: We will work in polygon local space and transform the points to world space after we are done.
            minPoint = transformMatrix.MultiplyPointInverse(_polygonPointsOnSamePlane[0]);
            maxPoint = transformMatrix.MultiplyPointInverse(_polygonPointsOnSamePlane[0]);
            for(int ptIndex = 0; ptIndex < _polygonPointsOnSamePlane.Count; ++ptIndex)
            {
                Vector3 point = transformMatrix.MultiplyPointInverse(_polygonPointsOnSamePlane[ptIndex]);
                if (point.x < minPoint.x) minPoint = point;
                if (point.x > maxPoint.x) maxPoint = point;
            }

            minPoint = transformMatrix.MultiplyPoint(minPoint);
            maxPoint = transformMatrix.MultiplyPoint(maxPoint);
        }

        private void QuickHull(List<Vector3> workingPoints, List<Segment3D> hullSegments)
        {
            // First, remove all points which lie inside the hull
            for (int pointIndex = 0; pointIndex < workingPoints.Count; )
            {
                Vector3 point = workingPoints[pointIndex];
                if (IsPointInsideHull(hullSegments, point))  workingPoints.RemoveAt(pointIndex);
                else ++pointIndex;
            }
  
            // Loop thorugh each existing hull segment and create new segments as necessary 
            var newSegments = new List<Segment3D>();
            for(int segmentIndex = 0; segmentIndex < hullSegments.Count;)
            {
                Segment3D currentSegment = hullSegments[segmentIndex];
                Plane currentSegmentPlane = GetSegmentPlane(currentSegment);

                // Expand the hull by creating new segments if necessary
                int indexOfFurthestPointInFront = currentSegmentPlane.GetIndexOfFurthestPointInFront(workingPoints);
                if (indexOfFurthestPointInFront >= 0)
                {
                    Vector3 furthestPointInFront = workingPoints[indexOfFurthestPointInFront];
                    hullSegments.RemoveAt(segmentIndex);

                    Segment3D firstSegment = new Segment3D(currentSegment.StartPoint, furthestPointInFront);
                    Segment3D secondSegment = new Segment3D(furthestPointInFront, currentSegment.EndPoint);

                    newSegments.Add(firstSegment);
                    newSegments.Add(secondSegment);

                    workingPoints.RemoveAt(indexOfFurthestPointInFront);
                }
                else ++segmentIndex;
            }

            if (newSegments.Count != 0)
            {
                hullSegments.AddRange(newSegments);
                QuickHull(workingPoints, hullSegments);
            }
        }

        private bool IsPointInsideHull(List<Segment3D> currentHullSegments, Vector3 point)
        {
            for (int segmentIndex = 0; segmentIndex < currentHullSegments.Count; ++segmentIndex)
            {
                Segment3D currentSegment = currentHullSegments[segmentIndex];
                if (!IsPointBehindSegmentPlane(currentSegment, point)) return false;
            }

            return true;
        }

        private bool IsPointBehindSegmentPlane(Segment3D segment, Vector3 point)
        {
            Plane segmentPlane = GetSegmentPlane(segment);
            return segmentPlane.IsPointBehind(point);
        }

        private Plane GetSegmentPlane(Segment3D segment)
        {
            Vector3 segmentPlaneNormal = Vector3.Cross(segment.Direction, _polygonNormal);
            segmentPlaneNormal.Normalize();

            return new Plane(segmentPlaneNormal, segment.StartPoint);
        }
        #endregion
    }
}
#endif