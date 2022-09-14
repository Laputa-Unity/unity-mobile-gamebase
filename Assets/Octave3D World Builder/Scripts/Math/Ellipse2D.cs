#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public struct Ellipse2D
    {
        #region Private Variables
        private Vector2 _center;
        private Vector2 _radii;
        #endregion

        #region Public Properties
        public Vector2 Center { get { return _center; } set { _center = value; } }
        public Vector2 Radii { get { return _radii; } set { _radii = value; } }
        public float RadiusX { get { return _radii.x; } set { _radii.x = value; } }
        public float RadiusY { get { return _radii.y; } set { _radii.y = value; } }
        #endregion

        #region Constructors
        public Ellipse2D(Vector2 center, Vector2 radii)
        {
            _center = center;
            _radii = radii;
        }

        public Ellipse2D(Vector2 center, float radiusX, float radiusY)
        {
            _center = center;
            _radii = new Vector2(radiusX, radiusY);
        }

        public Ellipse2D(Rect enclosingRectangle)
        {
            _center = enclosingRectangle.center;
            _radii = new Vector2(enclosingRectangle.width * 0.5f, enclosingRectangle.height * 0.5f);
        }

        public Ellipse2D(Circle2D circle)
        {
            _center = circle.Center;
            _radii = new Vector2(circle.Radius, circle.Radius);
        }
        #endregion

        #region Public Methods
        public bool OverlapsRectangle(Rect rectangle)
        {
            if (ContainsAnyPoint(rectangle.GetCenterAndCornerPoints())) return true;
            if (rectangle.ContainsAnyPoint(GetExtentsPoints())) return true;
            if (IntersectsAnyRay(rectangle.GetEdgeRays())) return true;

            return false;
        }

        public bool ContainsRectangle(Rect rectangle)
        {
            return ContainsAllPoints(rectangle.GetCornerPoints());
        }

        public bool ContainsAllPoints(List<Vector2> points)
        {
            Vector2 invRadii = new Vector2(1.0f / Radii.x, 1.0f / Radii.y);
            Circle2D circle = new Circle2D(Vector2.Scale(Center, invRadii), 1.0f);

            foreach (Vector2 point in points)
            {
                Vector2 adjustedPoint = Vector2.Scale(point, invRadii);
                if (!circle.ContainsPoint(adjustedPoint)) return false;
            }

            return true;
        }

        public bool ContainsAnyPoint(List<Vector2> points)
        {
            Vector2 invRadii = new Vector2(1.0f / Radii.x, 1.0f / Radii.y);
            Circle2D circle = new Circle2D(Vector2.Scale(Center, invRadii), 1.0f);

            foreach (Vector2 point in points)
            {
                Vector2 adjustedPoint = Vector2.Scale(point, invRadii);
                if (circle.ContainsPoint(adjustedPoint)) return true;
            }

            return false;
        }

        public bool ContainsPoint(Vector2 point)
        {
            Vector2 invRadii = new Vector2(1.0f / Radii.x, 1.0f / Radii.y);
            Circle2D circle = new Circle2D(Vector2.Scale(Center, invRadii), 1.0f);
    
            Vector2 adjustedPoint = Vector2.Scale(point, invRadii);
            return circle.ContainsPoint(adjustedPoint);
        }

        public bool IntersectsAnyRay(List<Ray2D> rays)
        {
            foreach(Ray2D ray in rays)
            {
                if (IntersectsRay(ray)) return true;
            }

            return false;
        }

        public bool IntersectsRay(Ray2D ray, out float t)
        {
            Vector2 invRadii = new Vector2(1.0f / Radii.x, 1.0f / Radii.y);
            Ray2D adjustedRay = new Ray2D(Vector2.Scale(ray.Origin, invRadii), Vector2.Scale(ray.Direction, invRadii));

            Circle2D circle = new Circle2D(Vector2.Scale(Center, invRadii), 1.0f);
            return circle.Raycast(adjustedRay, out t);
        }

        public bool IntersectsRay(Ray2D ray)
        {
            float t;
            return IntersectsRay(ray, out t);
        }

        public List<Vector2> GetExtentsPoints()
        {
            // Return the extent points in the following format: top, right, bottom, left.
            return new List<Vector2>
            {
                _center + new Vector2(0.0f, _radii.y),
                _center + new Vector2(_radii.x, 0.0f),
                _center + new Vector2(0.0f, -_radii.y),
                _center + new Vector2(-_radii.x, 0.0f)
            };
        }

        public Vector3 GetWorldCenterPointInFrontOfCamera(Camera camera, float distanceFromCamNearPlane)
        {
            return _center.GetWorldPointInFrontOfCamera(camera, distanceFromCamNearPlane);
        }

        public List<Vector3> GetWorldExtentPointsInFrontOfCamera(Camera camera, float distanceFromCamNearPlane)
        {
            List<Vector2> extentPoints = GetExtentsPoints();
            return Vector2Extensions.GetWorldPointsInFrontOfCamera(extentPoints, camera, distanceFromCamNearPlane);
        }

        public List<Vector3> GetWorldBorderLinesInFrontOfCamera(Camera camera, float distanceFromCamNearPlane, int numberOfEllipseSlices)
        {
            List<Vector3> worldExtentPoints = GetWorldExtentPointsInFrontOfCamera(camera, distanceFromCamNearPlane);

            Vector3 worldCenterPoint = GetWorldCenterPointInFrontOfCamera(camera, distanceFromCamNearPlane);
            Vector3 ellipseUp = worldExtentPoints[0] - worldCenterPoint;
            Vector3 ellipseRight = worldExtentPoints[1] - worldCenterPoint;

            float radiusX = ellipseRight.magnitude;
            float radiusY = ellipseUp.magnitude;

            ellipseUp.Normalize();
            ellipseRight.Normalize();

            return Vector3Extensions.Get3DEllipseCircumferencePoints(radiusX, radiusY, ellipseRight, ellipseUp, worldCenterPoint, numberOfEllipseSlices);
        }

        public bool FullyContainsOrientedBoxCenterAndCornerPointsInScreenSpace(OrientedBox orientedBox, Camera camera)
        {
            List<Vector2> boxCenterAndCornerScreenPoints = orientedBox.GetScreenCenterAndCornerPoints(camera);
            return ContainsAllPoints(boxCenterAndCornerScreenPoints);
        }
        #endregion
    }
}
#endif