#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class PlaneExtensions
    {
        #region Extension Methods
        public static Plane Transform(this Plane plane, Matrix4x4 transformMatrix, Vector3 pointOnPlane)
        {
            Vector3 transformedPoint = transformMatrix.MultiplyPoint(pointOnPlane);
            Vector3 transformedNormal = transformMatrix.MultiplyVector(plane.normal);
            transformedNormal.Normalize();

            return new Plane(transformedNormal, transformedPoint);
        }

        public static Vector3 ProjectPoint(this Plane plane, Vector3 pointToProject)
        {
            float planeDistanceFromPoint = plane.GetDistanceToPoint(pointToProject);
            return pointToProject - plane.normal * planeDistanceFromPoint;
        }

        public static List<Vector3> ProjectAllPoints(this Plane plane, List<Vector3> pointsToProject)
        {
            if (pointsToProject.Count == 0) return new List<Vector3>();
            var projectedPoints = new List<Vector3>(pointsToProject.Count);

            foreach(Vector3 point in pointsToProject)
            {
                projectedPoints.Add(plane.ProjectPoint(point));
            }

            return projectedPoints;
        }

        public static bool AreAllPointsInFrontOrOnPlane(this Plane plane, List<Vector3> points)
        {
            foreach(Vector3 point in points)
            {
                if (plane.IsPointBehind(point)) return false;
            }

            return true;
        }

        public static bool AreAllPointsInFrontOrBehindPlane(this Plane plane, List<Vector3> points)
        {
            bool foundPointInFront = false;
            bool foundPointBehind = false;
            foreach (Vector3 point in points)
            {
                if (plane.IsPointOnPlane(point)) return false;

                if(plane.IsPointInFront(point))
                {
                    if (foundPointBehind) return false;
                    foundPointInFront = true;
                }
                else
                if(plane.IsPointBehind(point))
                {
                    if (foundPointInFront) return false;
                    foundPointBehind = true;
                }
            }

            return true;
        }

        public static List<Vector3> GetAllPointsInFront(this Plane plane, List<Vector3> points)
        {
            if(points.Count == 0) return new List<Vector3>();

            var allPointsInFront = new List<Vector3>(points.Count);
            foreach(Vector3 point in points)
            {
                if(plane.IsPointInFront(point)) allPointsInFront.Add(point);
            }

            return allPointsInFront;
        }

        public static List<Vector3> GetAllPointsBehind(this Plane plane, List<Vector3> points)
        {
            if(points.Count == 0) return new List<Vector3>();

            var allPointsBehind = new List<Vector3>(points.Count);
            foreach(Vector3 point in points)
            {
                if(plane.IsPointBehind(point)) allPointsBehind.Add(point);
            }

            return allPointsBehind;
        }

        public static Vector3 GetClosestPointToPlane(this Plane plane, List<Vector3> points)
        {
            float minDistanceToPoint = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;

            foreach(Vector3 point in points)
            {
                float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                if(planeDistanceToPoint < minDistanceToPoint)
                {
                    minDistanceToPoint = planeDistanceToPoint;
                    closestPoint = point;
                }
            }

            return closestPoint;
        }

        public static bool GetClosestPointInFront(this Plane plane, List<Vector3> points, out Vector3 closestPointInFront)
        {
            float minDistanceToPoint = float.MaxValue;
            closestPointInFront = Vector3.zero;
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if (plane.IsPointInFront(point))
                {
                    foundPoint = true;
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));

                    if (planeDistanceToPoint < minDistanceToPoint)
                    {
                        minDistanceToPoint = planeDistanceToPoint;
                        closestPointInFront = point;
                    }
                }
            }

            return foundPoint;
        }

        public static bool GetClosestPointBehind(this Plane plane, List<Vector3> points, out Vector3 closestPointBehind)
        {
            float minDistanceToPoint = float.MaxValue;
            closestPointBehind = Vector3.zero;
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if (plane.IsPointBehind(point))
                {
                    foundPoint = true;
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));

                    if (planeDistanceToPoint < minDistanceToPoint)
                    {
                        minDistanceToPoint = planeDistanceToPoint;
                        closestPointBehind = point;
                    }
                }
            }

            return foundPoint;
        }

        public static Vector3 GetFurthestPointFromPlane(this Plane plane, List<Vector3> points)
        {
            float maxDistanceToPoint = float.MinValue;
            Vector3 furthestPoint = Vector3.zero;

            foreach (Vector3 point in points)
            {
                float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                if (planeDistanceToPoint > maxDistanceToPoint)
                {
                    maxDistanceToPoint = planeDistanceToPoint;
                    furthestPoint = point;
                }
            }

            return furthestPoint;
        }

        public static bool GetFurthestPointInFront(this Plane plane, List<Vector3> points, out Vector3 furthestPointInFront)
        {
            float maxDistanceToPoint = float.MinValue;
            furthestPointInFront = Vector3.zero;
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if (plane.IsPointInFront(point))
                {
                    foundPoint = true;
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));

                    if (planeDistanceToPoint > maxDistanceToPoint)
                    {
                        maxDistanceToPoint = planeDistanceToPoint;
                        furthestPointInFront = point;
                    }
                }
            }

            return foundPoint;
        }

        public static int GetIndexOfFurthestPointInFront(this Plane plane, List<Vector3> points)
        {
            float maxDistanceToPoint = float.MinValue;
            int indexOfFurthestPointInFront = -1;

            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                if (plane.IsPointInFront(point))
                {
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                    if (planeDistanceToPoint > maxDistanceToPoint)
                    {
                        maxDistanceToPoint = planeDistanceToPoint;
                        indexOfFurthestPointInFront = pointIndex;
                    }
                }
            }

            return indexOfFurthestPointInFront;
        }

        public static int GetIndexOfFurthestPointBehind(this Plane plane, List<Vector3> points)
        {
            float maxDistanceToPoint = float.MinValue;
            int indexOfFurthestPointBehind = -1;

            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                if (plane.IsPointBehind(point))
                {
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                    if (planeDistanceToPoint > maxDistanceToPoint)
                    {
                        maxDistanceToPoint = planeDistanceToPoint;
                        indexOfFurthestPointBehind = pointIndex;
                    }
                }
            }

            return indexOfFurthestPointBehind;
        }

        public static int GetIndexOfClosestPointInFront(this Plane plane, List<Vector3> points)
        {
            float minDistance = float.MaxValue;
            int ptIndex = -1;

            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                if (plane.IsPointInFront(point))
                {
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                    if (planeDistanceToPoint < minDistance)
                    {
                        minDistance = planeDistanceToPoint;
                        ptIndex = pointIndex;
                    }
                }
            }

            return ptIndex;
        }

        public static int GetIndexOfClosestPointInFrontOrOnPlane(this Plane plane, List<Vector3> points)
        {
            float minDistance = float.MaxValue;
            int ptIndex = -1;

            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                if (plane.IsPointOnPlane(point) || plane.IsPointInFront(point))
                {
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
                    if (planeDistanceToPoint < minDistance)
                    {
                        minDistance = planeDistanceToPoint;
                        ptIndex = pointIndex;
                    }
                }
            }

            return ptIndex;
        }

        public static bool GetFurthestPointBehind(this Plane plane, List<Vector3> points, out Vector3 furthestPointBehind)
        {
            float maxDistanceToPoint = float.MinValue;
            furthestPointBehind = Vector3.zero;
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if(plane.IsPointBehind(point))
                {
                    foundPoint = true;
                    float planeDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));

                    if (planeDistanceToPoint > maxDistanceToPoint)
                    {
                        maxDistanceToPoint = planeDistanceToPoint;
                        furthestPointBehind = point;
                    }
                }
            }

            return foundPoint;
        }

        public static bool GetFirstPointOnPlane(this Plane plane, List<Vector3> points, out Vector3 firstPointInPlane)
        {
            firstPointInPlane = Vector3.zero;
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if (plane.IsPointOnPlane(point))
                {
                    foundPoint = true;
                    firstPointInPlane = point;
                    break;
                }
            }
            
            return foundPoint;
        }

        public static float GetAbsDistanceToPoint(this Plane plane, Vector3 point)
        {
            return Mathf.Abs(plane.GetDistanceToPoint(point));
        }

        public static bool IsAnyPointOnPlane(this Plane plane, List<Vector3> points)
        {
            bool foundPoint = false;

            foreach (Vector3 point in points)
            {
                if (plane.IsPointOnPlane(point))
                {
                    foundPoint = true;
                    break;
                }
            }

            return foundPoint;
        }

        public static bool IsPointBehind(this Plane plane, Vector3 point)
        {
            return !plane.IsPointOnPlane(point) && plane.GetDistanceToPoint(point) < 0.0f;
        }

        public static bool IsPointInFront(this Plane plane, Vector3 point)
        {
            return !plane.IsPointOnPlane(point) && plane.GetDistanceToPoint(point) > 0.0f;
        }

        public static bool IsPointOnPlane(this Plane plane, Vector3 point, float epsilon = 1e-4f)
        {
            float absDistanceToPoint = Mathf.Abs(plane.GetDistanceToPoint(point));
            return absDistanceToPoint < epsilon;
        }
     
        public static Plane AdjustSoBoxSitsInFront(this Plane plane, OrientedBox orientedBox)
        {
            List<Vector3> boxCornerPoints = orientedBox.GetCornerPoints();
            Vector3 furthestPointBehind;

            if (plane.GetFurthestPointBehind(boxCornerPoints, out furthestPointBehind)) return new Plane(plane.normal, furthestPointBehind);
            else 
            {
                Vector3 closestPointInFront, pointOnPlane;

                if (plane.GetFirstPointOnPlane(boxCornerPoints, out pointOnPlane)) return new Plane(plane.normal, pointOnPlane);
                if (plane.GetClosestPointInFront(boxCornerPoints, out closestPointInFront)) return new Plane(plane.normal, closestPointInFront);
            }

            // If there are no points behind or in front, it means all points are on the plane, so we return the umodified plane instance
            return plane;
        }

        public static Plane AdjustSoBoxSitsBehind(this Plane plane, OrientedBox orientedBox)
        {
            List<Vector3> boxCornerPoints = orientedBox.GetCornerPoints();
            Vector3 furthestPointInFront;

            if (plane.GetFurthestPointInFront(boxCornerPoints, out furthestPointInFront)) return new Plane(plane.normal, furthestPointInFront);
            else
            {
                Vector3 closestPointBehind, pointOnPlane;

                if (plane.GetFirstPointOnPlane(boxCornerPoints, out pointOnPlane)) return new Plane(plane.normal, pointOnPlane);
                if (plane.GetClosestPointBehind(boxCornerPoints, out closestPointBehind)) return new Plane(plane.normal, closestPointBehind);
            }

            // If there are no points behind or in front, it means all points are on the plane, so we return the umodified plane instance
            return plane;
        }

        public static PointPlaneClassificationResult ClassifyPoint(this Plane plane, Vector3 point)
        {
            if (plane.IsPointOnPlane(point)) return PointPlaneClassificationResult.OnPlane;
            if (plane.IsPointBehind(point)) return PointPlaneClassificationResult.Behind;
            return PointPlaneClassificationResult.InFront;
        }

        public static BoxPlaneClassificationResult ClassifyOrientedBox(this Plane plane, OrientedBox orientedBox)
        {
            List<Vector3> boxCenterAndCornerPoints = orientedBox.GetCenterAndCornerPoints();

            bool foundPointInFront = false;
            bool foundPointBehind = false;
            bool foundPointOnPlane = false;
            foreach(Vector3 boxPoint in boxCenterAndCornerPoints)
            {
                PointPlaneClassificationResult pointClassificationResult = plane.ClassifyPoint(boxPoint);
                if (pointClassificationResult == PointPlaneClassificationResult.InFront)
                {
                    foundPointInFront = true;
                    if (foundPointBehind) return BoxPlaneClassificationResult.Spanning;
                }
                else
                if (pointClassificationResult == PointPlaneClassificationResult.Behind)
                {
                    foundPointBehind = true;
                    if (foundPointInFront) return BoxPlaneClassificationResult.Spanning;
                }
                else foundPointOnPlane = true;
            }

            if(foundPointOnPlane && (!foundPointInFront && !foundPointBehind)) return BoxPlaneClassificationResult.OnPlane;
            else
            {
                if (foundPointInFront) return BoxPlaneClassificationResult.InFront;
                return BoxPlaneClassificationResult.Behind;
            }
        }

        public static BoxPlaneClassificationResult ClassifyBox(this Plane plane, Box box)
        {
            List<Vector3> boxCenterAndCornerPoints = box.GetCenterAndCornerPoints();

            bool foundPointInFront = false;
            bool foundPointBehind = false;
            bool foundPointOnPlane = false;
            foreach (Vector3 boxPoint in boxCenterAndCornerPoints)
            {
                PointPlaneClassificationResult pointClassificationResult = plane.ClassifyPoint(boxPoint);
                if (pointClassificationResult == PointPlaneClassificationResult.InFront)
                {
                    foundPointInFront = true;
                    if (foundPointBehind) return BoxPlaneClassificationResult.Spanning;
                }
                else
                if (pointClassificationResult == PointPlaneClassificationResult.Behind)
                {
                    foundPointBehind = true;
                    if (foundPointInFront) return BoxPlaneClassificationResult.Spanning;
                }
                else foundPointOnPlane = true;
            }

            if (foundPointOnPlane && (!foundPointInFront && !foundPointBehind)) return BoxPlaneClassificationResult.OnPlane;
            else
            {
                if (foundPointInFront) return BoxPlaneClassificationResult.InFront;
                return BoxPlaneClassificationResult.Behind;
            }
        }
        #endregion

        #region Utilities
        public static Plane GetPlaneWhichFacesNormal(List<Plane> planes, Vector3 normal, out int planeIndex)
        {
            if(planes.Count == 0)
            {
                planeIndex = -1;
                return new Plane();
            }

            normal.Normalize();

            float bestAlignment = 1.0f;
            planeIndex = -1;

            for (int plIndex = 0; plIndex < planes.Count; ++plIndex)
            {
                Plane plane = planes[plIndex];
                float alignment = Vector3.Dot(plane.normal, normal);

                if(alignment < 0.0f && alignment < bestAlignment)
                {
                    bestAlignment = alignment;
                    planeIndex = plIndex;

                    if (Mathf.Abs(alignment + 1.0f) < 1e-4f) break;
                }
            }

            return planes[planeIndex];
        }

        public static Plane GetPlaneMostAlignedWithNormal(List<Plane> planes, Vector3 normal, out int planeIndex)
        {
            if (planes.Count == 0)
            {
                planeIndex = -1;
                return new Plane();
            }

            normal.Normalize();

            float bestAlignment = -1.0f;
            planeIndex = -1;

            for (int plIndex = 0; plIndex < planes.Count; ++plIndex)
            {
                Plane plane = planes[plIndex];
                float alignment = Vector3.Dot(plane.normal, normal);

                if (alignment > 0.0f && alignment > bestAlignment)
                {
                    bestAlignment = alignment;
                    planeIndex = plIndex;

                    if (Mathf.Abs(alignment - 1.0f) < 1e-4f) break;
                }
            }

            return planes[planeIndex];
        }

        public static int GetIndexOfPlaneWhoseNormalIsMostAlignedWithDir(List<Plane> planes, Vector3 directionVector)
        {
            int planeIndex = -1;
            if (planes.Count == 0) return planeIndex;

            directionVector.Normalize();

            float bestAlignment = -1.0f;
            for (int plIndex = 0; plIndex < planes.Count; ++plIndex)
            {
                Plane plane = planes[plIndex];
                float alignment = Vector3.Dot(plane.normal, directionVector);

                if (alignment > 0.0f && alignment > bestAlignment)
                {
                    bestAlignment = alignment;
                    planeIndex = plIndex;

                    if (plane.normal.IsAlignedWith(directionVector)) return planeIndex;
                }
            }

            return planeIndex;
        }
        #endregion
    }
}
#endif