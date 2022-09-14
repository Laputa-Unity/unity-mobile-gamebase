#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Vector2Extensions
    {
        #region Extension Methods
        public static Vector2 GetInverse(this Vector2 vector)
        {
            return new Vector2(1.0f / vector.x, 1.0f / vector.y);
        }

        public static Vector2 GetVectorWithAbsoluteValueComponents(this Vector2 vector)
        {
            vector.x = Mathf.Abs(vector.x);
            vector.y = Mathf.Abs(vector.y);

            return vector;
        }

        public static float GetPointDistanceFromSegment(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool treatSegmentAsInfiniteLine = false)
        {
            Vector2 segment = segmentEnd - segmentStart;
            float segmentLength = segment.magnitude;
            segment.Normalize();

            Vector2 fromSegmentStartToPoint = point - segmentStart;
            float pointProjectionOnSegment = Vector2.Dot(segment, fromSegmentStartToPoint);

            if (!treatSegmentAsInfiniteLine)
            {
                if (pointProjectionOnSegment >= 0 && pointProjectionOnSegment <= segmentLength)
                {
                    Vector2 projectedPoint = segmentStart + segment * pointProjectionOnSegment;
                    return (point - projectedPoint).magnitude;
                }

                if (pointProjectionOnSegment > segmentLength) return (point - segmentEnd).magnitude;
                else return (point - segmentStart).magnitude;
            }
            else
            {
                Vector2 projectedPoint = segmentStart + segment * pointProjectionOnSegment;
                return (point - projectedPoint).magnitude;
            }
        }

        public static Vector3 GetWorldPointInFrontOfCamera(this Vector2 point2D, Camera camera, float distanceFromCamNearPlane)
        {
            point2D.y = SceneView.lastActiveSceneView.camera.pixelRect.height - point2D.y;

            var ray = HandleUtility.GUIPointToWorldRay(point2D);
            Vector3 camLook = camera.transform.forward;
            Plane camNearPlane = new Plane(camLook, camera.transform.position + camLook * (camera.nearClipPlane + distanceFromCamNearPlane));

            float t;
            camNearPlane.Raycast(ray, out t);
            return ray.GetPoint(t);
        }
        #endregion

        #region Utilities
        public static List<Vector3> GetWorldPointsInFrontOfCamera(List<Vector2> points2D, Camera camera, float distanceFromCamNearPlane)
        {
            var pointsInFrontOfCamera = new List<Vector3>(points2D.Count);
            foreach(Vector2 point2D in points2D)
            {
                pointsInFrontOfCamera.Add(GetWorldPointInFrontOfCamera(point2D, camera, distanceFromCamNearPlane));
            }

            return pointsInFrontOfCamera;
        }

        public static List<Vector2> GetScreenPoints(List<Vector3> worldPoints, Camera camera)
        {
            if (worldPoints.Count == 0) return new List<Vector2>();
            var screenPoints = new List<Vector2>(worldPoints.Count);

            foreach (Vector3 worldPoint in worldPoints)
            {
                screenPoints.Add(Vector3Extensions.WorldToScreenPoint(worldPoint));
            }

            return screenPoints;
        }

        public static Box GetBoxFromPointCloud(List<Vector2> pointCloud)
        {
            if (pointCloud.Count == 0) return Box.GetInvalid();

            Vector3 min = pointCloud[0];
            Vector3 max = pointCloud[0];

            for(int ptIndex = 1; ptIndex < pointCloud.Count; ++ptIndex)
            {
                Vector3 point = pointCloud[ptIndex];
                min = Vector3.Min(point, min);
                max = Vector3.Max(point, max);
            }

            return new Box((min + max) * 0.5f, (max - min));
        }
        #endregion
    }
}
#endif