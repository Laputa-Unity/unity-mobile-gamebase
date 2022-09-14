#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class XZOrientedEllipse3D
    {
        #region Private Variables
        private float _radiusX = 1.0f;
        private float _radiusZ = 1.0f;
        private CoordinateSystem _coordinateSystem = new CoordinateSystem();
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return Vector3.right; } }
        public static Vector3 ModelSpacePlaneNormal { get { return Vector3.up; } }
        public static Vector3 ModelSpaceLookAxis { get { return Vector3.forward; } }
        #endregion

        #region Public Properties
        public float ModelSpaceRadiusX { get { return _radiusX; } set { _radiusX = Mathf.Abs(value); } }
        public float ModelSpaceRadiusZ { get { return _radiusZ; } set { _radiusZ = Mathf.Abs(value); } }
        public Vector2 ModelSpaceRadii { get { return new Vector2(_radiusX, _radiusZ); } set { ModelSpaceRadiusX = value.x; ModelSpaceRadiusZ = value.y; } }
        public float ScaledRadiusX { get { return _radiusX * _coordinateSystem.TransformMatrix.XScale; } }
        public float ScaledRadiusZ { get { return _radiusZ * _coordinateSystem.TransformMatrix.ZScale; } }
        public Vector2 ScaledRadii { get { return new Vector2(_radiusX * _coordinateSystem.TransformMatrix.XScale, _radiusZ * _coordinateSystem.TransformMatrix.ZScale); } }
        public Vector3 Center
        {
            get { return _coordinateSystem.GetOriginPosition(); }
            set { _coordinateSystem.SetOriginPosition(value); }
        }
        public Plane Plane { get { return new Plane(_coordinateSystem.GetAxisVector(CoordinateSystemAxis.PositiveUp), Center); } }
        public Vector3 Normal { get { return Plane.normal; } }
        public TransformMatrix TransformMatrix { get { return _coordinateSystem.TransformMatrix; } }
        public Quaternion Rotation { get { return _coordinateSystem.GetRotation(); } set { _coordinateSystem.SetRotation(value); } }
        #endregion

        #region Constructors
        public XZOrientedEllipse3D()
        {
        }

        public XZOrientedEllipse3D(Vector3 center)
        {
            Center = center;
        }

        public XZOrientedEllipse3D(Vector3 center, float radius)
        {
            Center = center;
            ModelSpaceRadiusX = radius;
            ModelSpaceRadiusZ = radius;
        }

        public XZOrientedEllipse3D(Vector3 center, float radiusX, float radiusZ)
        {
            Center = center;
            ModelSpaceRadiusX = radiusX;
            ModelSpaceRadiusZ = radiusZ;
        }

        public XZOrientedEllipse3D(Vector3 center, float radiusX, float radiusZ, Quaternion rotation)
        {
            Center = center;
            ModelSpaceRadiusX = radiusX;
            ModelSpaceRadiusZ = radiusZ;
            Rotation = rotation;
        }

        public XZOrientedEllipse3D(XZOrientedEllipse3D source)
        {
            Center = source.Center;
            ModelSpaceRadiusX = source.ModelSpaceRadiusX;
            ModelSpaceRadiusZ = source.ModelSpaceRadiusZ;
            Rotation = source.Rotation;
        }
        #endregion

        #region Public Methods
        public Vector3 GetRandomPointInside()
        {
            float randomAngleInRadians = UnityEngine.Random.Range(0.0f, Mathf.PI * 2.0f);
            float randomRadiusX = UnityEngine.Random.Range(0.0f, ScaledRadiusX);
            float randomRadiusZ = UnityEngine.Random.Range(0.0f, ScaledRadiusZ);

            // Searching the net, it seems that using the square root produces a more random distribution :)
            return Center + GetLocalAxis(CoordinateSystemAxis.PositiveRight) * Mathf.Sqrt(randomRadiusX) * Mathf.Cos(randomAngleInRadians) + 
                            GetLocalAxis(CoordinateSystemAxis.PositiveLook) * Mathf.Sqrt(randomRadiusZ) * Mathf.Sin(randomAngleInRadians);
        }

        public void SetScale(float scale)
        {
            _coordinateSystem.SetScaleOnAllAxes(scale);
        }

        public bool OverlapsPolygon(Polygon3D polygon)
        {
            if (ContainsAnyPoint(polygon.PointsOnPolygonPlane)) return true;
            if (polygon.ContainsAnyPoint(GetExtentsPoints())) return true;
            if (IntersectsAnyRay(polygon.GetEdgeRays())) return true;

            return false;
        }

        public bool IntersectsAnyRay(List<Ray3D> rays)
        {
            foreach (Ray3D ray in rays)
            {
                if (IntersectsRay(ray)) return true;
            }

            return false;
        }

        public bool IntersectsRay(Ray3D ray, out float t)
        {
            Ray3D circleSpaceRay = TransformRayInEllipseCircleSpace(ray);
            XZOrientedCircle3D circle = TransformToCircleLocalSpace();
      
            return circle.IntersectsRay(circleSpaceRay, out t);
        }

        public bool IntersectsRay(Ray3D ray)
        {
            float t;
            return IntersectsRay(ray, out t);
        }

        public bool ContainsAnyPoint(List<Vector3> points)
        {
            XZOrientedCircle3D circle = TransformToCircleLocalSpace();
            foreach (Vector3 point in points)
            {
                if (circle.ContainsPoint(TransformPointInEllipseCircleLocalSpace(point))) return true;
            }

            return false;
        }

        public bool ContainsAllPoints(List<Vector3> points)
        {
            XZOrientedCircle3D circle = TransformToCircleLocalSpace();
            foreach (Vector3 point in points)
            {
                if (!circle.ContainsPoint(TransformPointInEllipseCircleLocalSpace(point))) return false;
            }

            return true;
        }

        public bool ContainsPoint(Vector3 point)
        {
            XZOrientedCircle3D circle = TransformToCircleLocalSpace();
            return circle.ContainsPoint(TransformPointInEllipseCircleLocalSpace(point));
        }

        public List<Vector3> GetExtentsPoints()
        {
            // First egenrate the points in model space in the following order: top, right, bottom, left.
            List<Vector3> points = new List<Vector3> { Vector3.forward * _radiusZ, Vector3.right * _radiusX, Vector3.forward * -_radiusZ, Vector3 .right * -_radiusX };

            // Transform the points and return them
            return Vector3Extensions.GetTransformedPoints(points, TransformMatrix.ToMatrix4x4x);
        }

        public void SetNormal(Vector3 normal)
        {
            if (normal.magnitude == 0.0f) return;

            normal.Normalize();
            Vector3 currentNormal = Plane.normal;

            bool pointsInSameDirection;
            if (normal.IsAlignedWith(currentNormal, out pointsInSameDirection)) Rotate(GetLocalAxis(CoordinateSystemAxis.PositiveRight), 180.0f);
            else
            {
                Vector3 rotationAxis = Vector3.Cross(currentNormal, normal);
                Rotate(rotationAxis, currentNormal.AngleWith(normal));
            }
        }

        public void Rotate(Vector3 rotationAxis, float angleInDegrees)
        {
            rotationAxis.Normalize();
            Quaternion rotationQuaternion = Quaternion.AngleAxis(angleInDegrees, rotationAxis);
            Rotation = rotationQuaternion * Rotation;
        }

        public Vector3 GetLocalAxis(CoordinateSystemAxis axis)
        {
            return _coordinateSystem.GetAxisVector(axis);
        }
        #endregion

        #region Private Methods
        private XZOrientedCircle3D TransformToCircleLocalSpace()
        {
            var circle = new XZOrientedCircle3D();
            circle.Rotation = Quaternion.identity;
            circle.SetScale(1.0f);
            circle.ModelSpaceRadius = 1.0f;
            circle.Center = Vector3.zero;

            return circle;
        }

        private Vector3 TransformPointInEllipseCircleLocalSpace(Vector3 vector)
        {
            TransformMatrix transformMatrix = TransformMatrix;
            vector = transformMatrix.MultiplyPointInverse(vector);
            vector.x /= _radiusX;
            vector.z /= _radiusZ;

            return vector;
        }

        private Vector3 TransformVectorInEllipseCircleLocalSpace(Vector3 vector)
        {
            TransformMatrix transformMatrix = TransformMatrix;
            vector = transformMatrix.MultiplyVectorInverse(vector);
            vector.x /= _radiusX;
            vector.z /= _radiusZ;

            return vector;
        }

        private Ray3D TransformRayInEllipseCircleSpace(Ray3D ray)
        {
            return new Ray3D(TransformPointInEllipseCircleLocalSpace(ray.Origin), TransformVectorInEllipseCircleLocalSpace(ray.Direction));
        }
        #endregion
    }
}
#endif