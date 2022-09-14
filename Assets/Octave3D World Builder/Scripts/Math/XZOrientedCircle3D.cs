#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class XZOrientedCircle3D
    {
        #region Private Variables
        private float _radius = 1.0f;
        private CoordinateSystem _coordinateSystem = new CoordinateSystem();
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return Vector3.right; } }
        public static Vector3 ModelSpacePlaneNormal { get { return Vector3.up; } }
        public static Vector3 ModelSpaceLookAxis { get { return Vector3.forward; } }
        #endregion

        #region Public Properties
        public float ModelSpaceRadius { get { return _radius; } set { _radius = Mathf.Abs(value); } }
        public float ScaledRadius { get { return _radius * _coordinateSystem.TransformMatrix.XScale; } }
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
        public XZOrientedCircle3D()
        {
        }

        public XZOrientedCircle3D(Vector3 center)
        {
            Center = center;
        }

        public XZOrientedCircle3D(Vector3 center, float radius)
        {
            Center = center;
            ModelSpaceRadius = radius;
        }

        public XZOrientedCircle3D(Vector3 center, float radius, Quaternion rotation)
        {
            Center = center;
            ModelSpaceRadius = radius;
            Rotation = rotation;
        }

        public XZOrientedCircle3D(XZOrientedCircle3D source)
        {
            Center = source.Center;
            ModelSpaceRadius = source.ModelSpaceRadius;
            Rotation = source.Rotation;
        }
        #endregion

        #region Public Methods
        public void SetScale(float scale)
        {
            _coordinateSystem.SetScaleOnAllAxes(scale);
        }

        public bool ContainsAnyPoint(List<Vector3> points)
        {
            foreach (Vector2 point in points)
            {
                if (ContainsPoint(point)) return true;
            }

            return false;
        }

        public bool ContainsPoint(Vector3 point)
        {
            if (!Plane.IsPointOnPlane(point)) return false;
            return (point - Center).magnitude <= _radius;
        }

        public List<Vector3> GetExtentsPoints()
        {
            Vector3 center = Center;
            Vector3 localRight = GetLocalAxis(CoordinateSystemAxis.PositiveRight);
            Vector3 localLook = GetLocalAxis(CoordinateSystemAxis.PositiveLook);
            float scaledRadius = ScaledRadius;

            // Return the extent points in the following format: top, right, bottom, left.
            return new List<Vector3>
            {
                center + localLook * scaledRadius,
                center + localRight * scaledRadius,
                center - localLook * scaledRadius,
                center - localRight * scaledRadius,
            };
        }

        public bool IntersectsRay(Ray3D ray, out float t)
        {
            Plane circlePlane = Plane;
            Vector3 circleCenter = Center;
            float scaledRadius = ScaledRadius;

            // Note: 'IntersectsPlane' not enough. Also check for containment????
            if (ray.IntersectsPlane(circlePlane, out t)) return true;
            else
            if (ray.Direction.IsPerpendicularTo(circlePlane.normal))
            {
                // Project the circle center onto the ray direction segment. If the distance between the circle center
                // and the projected center is <= the circle's radius, the ray might intersect the circle. Otherwise,
                // the ray can not possibly intersect the circle.
                Segment3D segment = new Segment3D(ray);
                Vector3 centerProjectedOnRayDir = circleCenter.CalculateProjectionPointOnSegment(segment.StartPoint, segment.EndPoint);
                Vector3 fromCenterToProjectedCenter = centerProjectedOnRayDir - circleCenter;
                float triAdjSideLength1 = fromCenterToProjectedCenter.magnitude;
                if (triAdjSideLength1 > scaledRadius) return false;
                
                // At this point it is possible that the ray might intersect the circle. Calcluate how much
                // we have to move from the center projection to a point on the circle along the reverse of 
                // the ray direction vector. We will store this amount in 'triAdjSideLength2'.
                float triAdjSideLength2 = Mathf.Sqrt(scaledRadius * scaledRadius - triAdjSideLength1 * triAdjSideLength1);

                // Now check if moving from the projected center along the reverse ray direction by an amount equal to
                // 'triAdjSideLength2', we end up on a point which resides on the ray direction segment.
                Vector3 normalizedRayDirection = ray.Direction;
                normalizedRayDirection.Normalize();
                Vector3 targetPoint = centerProjectedOnRayDir - triAdjSideLength2 * normalizedRayDirection;
                if (targetPoint.IsOnSegment(segment.StartPoint, segment.EndPoint))
                {
                    // The point sits on the segment, which means that the ray intersects the circle.
                    // Now we need to calculate the intersection offset.
                    t = (targetPoint - ray.Origin).magnitude / ray.Direction.magnitude;
                    return true;
                }
            }
         
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

        public bool IntersectsRay(Ray3D ray)
        {
            float t;
            return IntersectsRay(ray, out t);
        }

        public void SetNormal(Vector3 normal)
        {
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
    }
}
#endif