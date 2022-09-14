#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class RectExtensions
    {
        #region Extension Methods
        public static List<Ray2D> GetEdgeRays(this Rect rectangle)
        {
            List<Vector2> cornerPoints = rectangle.GetCornerPoints();
            return new List<Ray2D>
            {
                new Ray2D(cornerPoints[0], cornerPoints[1] - cornerPoints[0]),
                new Ray2D(cornerPoints[1], cornerPoints[2] - cornerPoints[1]),
                new Ray2D(cornerPoints[2], cornerPoints[3] - cornerPoints[2]),
                new Ray2D(cornerPoints[0], cornerPoints[0] - cornerPoints[3])
            };
        }

        public static List<Vector2> GetCornerPoints(this Rect rectangle)
        {
            // Return the points in the following format: bottom left, bottom right, top right, top left.
            return new List<Vector2>
            {
                new Vector2(rectangle.xMin, rectangle.yMin),
                new Vector2(rectangle.xMax, rectangle.yMin),
                new Vector2(rectangle.xMax, rectangle.yMax),
                new Vector2(rectangle.xMin, rectangle.yMax)
            };
        }

        public static List<Vector2> GetCenterAndCornerPoints(this Rect rectangle)
        {
            // Return the points in the following format: center, bottom left, bottom right, top right, top left.
            var points = new List<Vector2>();
            points.Add(rectangle.center);
            points.AddRange(rectangle.GetCornerPoints());

            return points;
        }

        public static List<Vector3> GetWorldCornerPointsInFrontOfCamera(this Rect rectangle, Camera camera, float distanceFromCamNearPlane)
        {
            List<Vector2> cornerPoints = rectangle.GetCornerPoints();
            return Vector2Extensions.GetWorldPointsInFrontOfCamera(cornerPoints, camera, distanceFromCamNearPlane);
        }

        public static Vector3 GetWorldCenterPointInFrontOfCamera(this Rect rectangle, Camera camera, float distanceFromCamNearPlane)
        {
            return rectangle.center.GetWorldPointInFrontOfCamera(camera, distanceFromCamNearPlane);
        }

        public static bool ContainsRectangle(this Rect rectangle, Rect queryRectangle)
        {
            return rectangle.ContainsAllPoints(queryRectangle.GetCornerPoints());
        }

        public static bool ContainsAllPoints(this Rect rectangle, List<Vector2> points)
        {
            foreach (Vector2 point in points)
            {
                if (!rectangle.Contains(point, true)) return false;
            }

            return true;
        }

        public static bool ContainsAnyPoint(this Rect rectangle, List<Vector2> points)
        {
            foreach (Vector2 point in points)
            {
                if (rectangle.Contains(point, true)) return true;
            }

            return false;
        }

        public static bool FullyContainsOrientedBoxCenterAndCornerPointsInScreenSpace(this Rect rectangle, OrientedBox orientedBox, Camera camera)
        {
            List<Vector2> boxCenterAndCornerScreenPoints = orientedBox.GetScreenCenterAndCornerPoints(camera);
            return rectangle.ContainsAllPoints(boxCenterAndCornerScreenPoints);
        }
        #endregion
    }
}
#endif