#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Vector3Extensions
    {
        #region Extension Methods
        public static bool HasZeroComponent(this Vector3 vector, float epsilon = 0.0f)
        {
            float absComp = Mathf.Abs(vector.x);
            if (absComp < epsilon) return true;

            absComp = Mathf.Abs(vector.y);
            if (absComp < epsilon) return true;

            absComp = Mathf.Abs(vector.z);
            if (absComp < epsilon) return true;

            return false;
        }

        public static Vector3 GetSignVector(this Vector3 vector)
        {
            return new Vector3(Mathf.Sign(vector.x), Mathf.Sign(vector.y), Mathf.Sign(vector.z));
        }

        public static Vector3 ReplaceCoordsValueWith(this Vector3 vector, float valueToReplace, float value)
        {
            if (vector.x == valueToReplace) vector.x = value;
            if (vector.y == valueToReplace) vector.y = value;
            if (vector.z == valueToReplace) vector.z = value;

            return vector;
        }

        public static float AngleWith(this Vector3 thisVector, Vector3 other)
        {
            thisVector.Normalize();
            other.Normalize();

            return Mathf.Rad2Deg * Mathf.Acos(Mathf.Clamp(Vector3.Dot(thisVector, other), -1.0f, 1.0f));
        }

        public static Vector3 GetVectorWithPositiveComponents(this Vector3 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);
            vector.z = Mathf.Abs(vector.z);

            return vector;
        }

        public static Vector3 GetInverse(this Vector3 vector)
        {
            return new Vector3(1.0f / vector.x, 1.0f / vector.y, 1.0f / vector.z);
        }

        public static float GetComponentWithBiggestAbsValue(this Vector3 vector)
        {
            float maxComponent = vector.x;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(vector.y)) maxComponent = vector.y;
            if (Mathf.Abs(maxComponent) < Mathf.Abs(vector.z)) maxComponent = vector.z;

            return maxComponent;
        }

        public static bool IsAlignedWith(this Vector3 vector, Vector3 otherVector)
        {
            vector.Normalize();
            otherVector.Normalize();

            float absDot = Mathf.Abs(Vector3.Dot(vector, otherVector));
            return Mathf.Abs(absDot - 1.0f) < 1e-5f;
        }

        public static bool IsAlignedWith(this Vector3 vector, Vector3 otherVector, out bool pointsInSameDirection)
        {
            pointsInSameDirection = false;

            vector.Normalize();
            otherVector.Normalize();

            float dotProduct = Vector3.Dot(vector, otherVector);
            float absDot = Mathf.Abs(dotProduct);

            if(Mathf.Abs(absDot - 1.0f) < 1e-5f)
            {
                pointsInSameDirection = dotProduct > 0.0f;
                return true;
            }

            return false;
        }

        public static bool IsPointingInSameGeneralDirection(this Vector3 vector, Vector3 otherVector)
        {
            return Vector3.Dot(vector, otherVector) > 0.0f;
        }

        public static bool IsPerpendicularTo(this Vector3 vector, Vector3 otherVector)
        {
            return vector.GetAbsDot(otherVector) < 1e-6f;
        }

        public static float GetAbsDot(this Vector3 vector, Vector3 otherVector)
        {
            return Mathf.Abs(Vector3.Dot(vector, otherVector));
        }

        public static Vector3 CalculateProjectionPointOnSegment(this Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 segmentDirection = segmentEnd - segmentStart;
            segmentDirection.Normalize();

            Vector3 fromStartPointToPoint = point - segmentStart;
            float dotProduct = Vector3.Dot(segmentDirection, fromStartPointToPoint);
            return segmentStart + segmentDirection * dotProduct;
        }

        public static float GetDistanceFromSegment(this Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 pointProjectionOnSegment = point.CalculateProjectionPointOnSegment(segmentStart, segmentEnd);
            return (pointProjectionOnSegment - point).magnitude;
        }

        public static bool IsOnSegment(this Vector3 point, Vector3 segmentStart, Vector3 segmentEnd)
        {
            Vector3 segmentDirection = segmentEnd - segmentStart;
            float segmentLength = segmentDirection.magnitude;
            segmentDirection.Normalize();
            Vector3 fromStartToPoint = point - segmentStart;

            float dotProduct = Vector3.Dot(fromStartToPoint, segmentDirection);
            if (dotProduct < 0.0f) return false;
            if (dotProduct > segmentLength) return false;

            return true;
        }

        public static int GetIndexOfMostAlignedVector(this Vector3 thisVector, List<Vector3> vectors)
        {
            int bestVectorIndex = -1;
            float bestScrore = -1.0f;

            Vector3 refVector = Vector3.Normalize(thisVector);
            for (int index = 0; index < vectors.Count; ++index)
            {
                Vector3 vec = Vector3.Normalize(vectors[index]);
                float dot = Vector3.Dot(refVector, vec);
                if (dot > bestScrore)
                {
                    bestScrore = dot;
                    bestVectorIndex = index;
                }
            }

            return bestVectorIndex;
        }
        #endregion

        #region Utilities
        public static Vector2 WorldToScreenPoint(Vector3 worldPoint)
        {
            Vector2 screenPt = HandleUtility.WorldToGUIPoint(worldPoint);
            screenPt.y = SceneView.lastActiveSceneView.camera.pixelRect.height - screenPt.y;

            return screenPt;
        }

        public static List<Vector3> Get3DRectangleCircumferencePoints(float rectangleWidth, float rectangleHeight, Vector3 rectangleWidthDirection, Vector3 rectangleHeightDirection, Vector3 rectangleCenter)
        {
            var rectanglePoints = new List<Vector3>(4);

            rectangleWidthDirection.Normalize();
            rectangleHeightDirection.Normalize();

            float halfWidth = rectangleWidth * 0.5f;
            float halfHeight = rectangleHeight * 0.5f;

            rectanglePoints.Add(rectangleCenter - rectangleWidthDirection * halfWidth + rectangleHeightDirection * halfHeight);
            rectanglePoints.Add(rectangleCenter + rectangleWidthDirection * halfWidth + rectangleHeightDirection * halfHeight);
            rectanglePoints.Add(rectangleCenter + rectangleWidthDirection * halfWidth - rectangleHeightDirection * halfHeight);
            rectanglePoints.Add(rectangleCenter - rectangleWidthDirection * halfWidth - rectangleHeightDirection * halfHeight);

            return rectanglePoints;
        }

        public static List<Vector3> Get3DEllipseCircumferencePoints(float radiusX, float radiusY, Vector3 ellipseXRadiusDirection, Vector3 ellipseYRadiusDirection, Vector3 ellipseCenter, int numberOfEllipseSlices)
        {
            if (numberOfEllipseSlices < 4) return new List<Vector3>();

            ellipseXRadiusDirection.Normalize();
            ellipseYRadiusDirection.Normalize();

            int numberOfCircumferenceVerts = numberOfEllipseSlices + 1;
            float angleStep = 360.0f / numberOfEllipseSlices;

            var ellipsePoints = new List<Vector3>(numberOfCircumferenceVerts);
            for (int vertexIndex = 0; vertexIndex < numberOfCircumferenceVerts; ++vertexIndex)
            {
                float angle = angleStep * vertexIndex * Mathf.Deg2Rad;
                ellipsePoints.Add(ellipseXRadiusDirection * Mathf.Sin(angle) * radiusX + ellipseYRadiusDirection * Mathf.Cos(angle) * radiusY + ellipseCenter);
            }

            return ellipsePoints;
        }

        public static void GetMinMaxPoints(List<Vector3> points, out Vector3 min, out Vector3 max)
        {
            min = points[0];
            max = points[0];

            for(int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                min = Vector3.Min(min, points[pointIndex]);
                max = Vector3.Max(max, points[pointIndex]);
            }
        }

        public static Box GetPointCloudBox(List<Vector3> points)
        {
            Vector3 minPoint, maxPoint;
            GetMinMaxPoints(points, out minPoint, out maxPoint);

            return new Box((minPoint + maxPoint) * 0.5f, (maxPoint - minPoint));
        }

        public static List<Vector3> GetTransformedPoints(List<Vector3> pointsToTransform, Matrix4x4 transformMatrix)
        {
            if (pointsToTransform.Count == 0) return new List<Vector3>();
            var transformedPoints = new List<Vector3>(pointsToTransform.Count);
            
            foreach(Vector3 point in pointsToTransform)
            {
                transformedPoints.Add(transformMatrix.MultiplyPoint(point));
            }

            return transformedPoints;
        }

        public static List<Vector3> ApplyOffsetToPoints(List<Vector3> pointsToOffset, Vector3 offsetVector)
        {
            if (pointsToOffset.Count == 0) return new List<Vector3>();
            var newPoints = new List<Vector3>(pointsToOffset.Count);

            foreach (Vector3 point in pointsToOffset)
            {
                newPoints.Add(point + offsetVector);
            }

            return newPoints;
        }

        public static List<Vector3> GetQuadCornerPointsFromQuadMinMax(Vector3 quadMin, Vector3 quadMax, Vector3 quadRight, Vector3 quadLook)
        {
            quadRight.Normalize();
            quadLook.Normalize();

            Vector3 quadDiagonal = quadMax - quadMin;
            float quadSizeX = Vector3.Dot(quadDiagonal, quadRight);
            float quadSizeZ = Vector3.Dot(quadDiagonal, quadLook);

            var quadPoints = new List<Vector3>(4);
            quadPoints.Add(quadMin);
            quadPoints.Add(quadMin + quadRight * quadSizeX);
            quadPoints.Add(quadMax);
            quadPoints.Add(quadMin + quadLook * quadSizeZ);

            return quadPoints;
        }

        public static Vector3 GetAveragePoint(List<Vector3> pointsToAverage)
        {
            Vector3 sum = Vector3.zero;
            foreach (Vector3 point in pointsToAverage)
            {
                sum += point;
            }

            if (pointsToAverage.Count != 0) return sum * (1.0f / pointsToAverage.Count);
            return Vector3.zero;
        }

        public static Vector3 GetClosestPointToPoint(List<Vector3> points, Vector3 point)
        {
            float minDistanceBetweenPoints = float.MaxValue;
            Vector3 closestPoint = Vector3.zero;

            foreach(Vector3 pt in points)
            {
                float distanceBetweenPoints = (pt - point).magnitude;
                if(distanceBetweenPoints < minDistanceBetweenPoints)
                {
                    minDistanceBetweenPoints = distanceBetweenPoints;
                    closestPoint = pt;
                }
            }

            return closestPoint;
        }

        public static int GetIndexOfPointFurthestFromSegment(List<Vector3> points, Vector3 segmentStart, Vector3 segmentEnd)
        {
            float maxDistance = float.MinValue;
            int indexOfPointFurthestFromSegment = -1;

            for (int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                Vector3 point = points[pointIndex];
                float distanceFromSegment = point.GetDistanceFromSegment(segmentStart, segmentEnd);

                if (distanceFromSegment > maxDistance)
                {
                    maxDistance = distanceFromSegment;
                    indexOfPointFurthestFromSegment = pointIndex;
                }
            }

            return indexOfPointFurthestFromSegment;
        }

        public static int[] GetIndicesOfClosest2Points(List<Vector3> firstPointList, List<Vector3> secondPointList)
        {
            float minPointDistance = float.MaxValue;
            int indexOfPointInFirstList = -1;
            int indexOfPointInSecondList = -1;

            for(int pointIndexInFirstList = 0; pointIndexInFirstList < firstPointList.Count; ++pointIndexInFirstList)
            {
                for(int pointIndexInSecondList = 0; pointIndexInSecondList < secondPointList.Count; ++pointIndexInSecondList)
                {
                    float distanceBetweenPoints = (firstPointList[pointIndexInFirstList] - secondPointList[pointIndexInSecondList]).magnitude;
                    if (distanceBetweenPoints < minPointDistance)
                    {
                        indexOfPointInFirstList = pointIndexInFirstList;
                        indexOfPointInSecondList = pointIndexInSecondList;
                        minPointDistance = distanceBetweenPoints;
                    }
                }
            }

            return new int[] { indexOfPointInFirstList, indexOfPointInSecondList };
        }

        public static int[] GetIndicesOfFurhtest2Points(List<Vector3> firstPointList, List<Vector3> secondPointList)
        {
            float maxPointDistance = float.MinValue;
            int indexOfPointInFirstList = -1;
            int indexOfPointInSecondList = -1;

            for (int pointIndexInFirstList = 0; pointIndexInFirstList < firstPointList.Count; ++pointIndexInFirstList)
            {
                for (int pointIndexInSecondList = 0; pointIndexInSecondList < secondPointList.Count; ++pointIndexInSecondList)
                {
                    float distanceBetweenPoints = (firstPointList[pointIndexInFirstList] - secondPointList[pointIndexInSecondList]).magnitude;
                    if (distanceBetweenPoints > maxPointDistance)
                    {
                        indexOfPointInFirstList = pointIndexInFirstList;
                        indexOfPointInSecondList = pointIndexInSecondList;
                        maxPointDistance = distanceBetweenPoints;
                    }
                }
            }

            return new int[] { indexOfPointInFirstList, indexOfPointInSecondList };
        }

        public static int GetIndexOfPointClosestToPoint(List<Vector3> points, Vector3 point)
        {
            float minDistance = float.MaxValue;
            int indexOfClosestPoint = -1;

            for(int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
            {
                float distanceBetweenPoints = (points[pointIndex] - point).magnitude;
                if(distanceBetweenPoints < minDistance)
                {
                    minDistance = distanceBetweenPoints;
                    indexOfClosestPoint = pointIndex;
                }
            }

            return indexOfClosestPoint;
        }

        public static List<Segment3D> GetSegmentsBetweenPoints(List<Vector3> points, bool createSegmentBetweenLastAndFirstPoint)
        {
            if (points.Count <= 1) return new List<Segment3D>();

            var segments = new List<Segment3D>(points.Count);
            if (createSegmentBetweenLastAndFirstPoint && points.Count > 2)
            {
                for(int pointIndex = 0; pointIndex < points.Count; ++pointIndex)
                {
                    Vector3 startPoint = points[pointIndex];
                    Vector3 endPoint = points[(pointIndex + 1) % points.Count];
                    segments.Add(new Segment3D(startPoint, endPoint));
                }
            }
            else
            {
                for (int pointIndex = 0; pointIndex < points.Count - 1; ++pointIndex)
                {
                    Vector3 startPoint = points[pointIndex];
                    Vector3 endPoint = points[pointIndex + 1];
                    segments.Add(new Segment3D(startPoint, endPoint));
                }
            }

            return segments;
        }

        public static void EliminateDuplicatePoints(List<Vector3> points)
        {
            int currentPointIndex = 0;
            while (currentPointIndex < points.Count)
            {
                Vector3 currentPoint = points[currentPointIndex];
                for (int searchPointIndex = currentPointIndex + 1; searchPointIndex < points.Count; )
                {
                    Vector3 searchPoint = points[searchPointIndex];
                    float distanceBetweenPoints = (searchPoint - currentPoint).magnitude;

                    if (distanceBetweenPoints < 1e-5f) points.RemoveAt(searchPointIndex);
                    else ++searchPointIndex;
                }

                ++currentPointIndex;
            }
        }

        public static int GetIndexOfMostAlignedVectorIgnoreSign(List<Vector3> vectors, Vector3 refVector)
        {
            int bestVectorIndex = -1;
            float bestScrore = -1.0f;

            refVector.Normalize();
            for(int index = 0; index < vectors.Count; ++index)
            {
                Vector3 vec = Vector3.Normalize(vectors[index]);
                float dot = vec.GetAbsDot(refVector);
                if(dot > bestScrore)
                {
                    bestScrore = dot;
                    bestVectorIndex = index;
                }
            }

            return bestVectorIndex;
        }
        #endregion
    }
}
#endif