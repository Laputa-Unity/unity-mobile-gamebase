#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public struct Sphere
    {
        #region Private Variables
        private float _radius;
        private Vector3 _center;
        #endregion

        #region Constructors
        public Sphere(Vector3 center)
        {
            _radius = 1.0f;
            _center = center;
        }

        public Sphere(float radius)
        {
            _radius = radius;
            _center = Vector3.zero;
        }

        public Sphere(Vector3 center, float radius)
        {
            _radius = radius;
            _center = center;
        }

        public Sphere(Sphere source)
        {
            _radius = source._radius;
            _center = source._center;
        }
        #endregion

        #region Public Properties
        public float Radius { get { return _radius; } set { _radius = value; } }
        public Vector3 Center { get { return _center; } set { _center = value; } }
        #endregion

        #region Public Methods
        public bool Raycast(Ray ray)
        {
            float t;
            return Raycast(ray, out t);
        }

        public bool Raycast(Ray ray, out float t)
        {
            t = 0.0f;

            // Calculate the coefficients of the quadratic equation
            Vector3 sphereCenterToRayOrigin = ray.origin - _center;
            float a = Vector3.SqrMagnitude(ray.direction);
            float b = 2.0f * Vector3.Dot(ray.direction, sphereCenterToRayOrigin);
            float c = Vector3.SqrMagnitude(sphereCenterToRayOrigin) - _radius * _radius;

            // If we have a solution to the equation, the ray most likely intersects the sphere.
            float t1, t2;
            if (Equation.SolveQuadratic(a, b, c, out t1, out t2))
            {
                // Make sure the ray doesn't intersect the sphere only from behind
                if (t1 < 0.0f && t2 < 0.0f) return false;

                // Make sure we are using the smallest positive t value
                if (t1 < 0.0f)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }
                t = t1;

                return true;
            }

            // If we reach this point it means the ray does not intersect the sphere in any way
            return false;
        }

        public float GetDistanceBetweenCenters(Sphere sphere)
        {
            return (_center - sphere.Center).magnitude;
        }

        public float GetDistanceBetweenCentersSq(Sphere sphere)
        {
            return (Center - sphere.Center).sqrMagnitude;
        }

        public bool FullyOverlaps(Sphere sphere)
        {
            return GetDistanceBetweenCenters(sphere) + sphere.Radius <= _radius;
        }

        public bool OverlapsFullyOrPartially(Sphere sphere)
        {
            float distanceBetweenCenters = GetDistanceBetweenCenters(sphere);

            // Fully?
            if (distanceBetweenCenters + sphere.Radius <= _radius) return true;

            // Partially?
            return distanceBetweenCenters - sphere.Radius <= _radius;
        }

        public bool OverlapsFullyOrPartially(OrientedBox orientedBox)
        {
            Vector3 closestPointToSphereCenter = orientedBox.GetClosestPointToPoint(_center);
            return (closestPointToSphereCenter - _center).sqrMagnitude <= _radius * _radius;
        }

        public Sphere Encapsulate(Sphere sphere)
        {
            float distanceBetweenCenters = GetDistanceBetweenCenters(sphere);

            float newDiameter = distanceBetweenCenters + _radius + sphere.Radius;
            float newRadius = newDiameter * 0.5f;

            Vector3 fromThisToOther = sphere.Center - Center;
            fromThisToOther.Normalize();
            Vector3 newCenter = Center - fromThisToOther * _radius + fromThisToOther * newRadius;

            return new Sphere(newCenter, newRadius);
        }
        #endregion
    }
}
#endif