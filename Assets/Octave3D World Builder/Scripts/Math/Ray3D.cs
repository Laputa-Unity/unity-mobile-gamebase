#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct Ray3D
    {
        #region Private Variables
        private Vector3 _origin;
        private Vector3 _direction;
        private Vector3 _normalizedDirection;
        #endregion

        #region Public Properties
        public Vector3 Origin { get { return _origin; } set { _origin = value; } }
        public Vector3 Direction 
        { 
            get { return _direction; } 
            set 
            { 
                _direction = value;
                _normalizedDirection = _direction;
                _normalizedDirection.Normalize();
            }
        }
        public Vector3 NormalizedDirection { get { return _normalizedDirection; } }
        #endregion

        #region Constructors
        public Ray3D(Vector3 origin, Vector3 direction)
        {
            _origin = origin;
            _direction = direction;

            _normalizedDirection = direction;
            _normalizedDirection.Normalize();
        }

        public Ray3D(Ray source)
        {
            _origin = source.origin;
            _direction = source.direction;

            _normalizedDirection = _direction;
            _normalizedDirection.Normalize();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Returns a 'Ray' instance which describes the ray.
        /// </summary>
        /// <remarks>
        /// 'Ray' instances always have a normalized direction vector, so the returned
        /// ray will contain a normalized version of the direction vector.
        /// </remarks>
        public Ray ToRayWithNormalizedDirection()
        {
            return new Ray(_origin, _direction);
        }

        public void Transform(Matrix4x4 transformMatrix)
        {
            _origin = transformMatrix.MultiplyPoint(_origin);
            Direction = transformMatrix.MultiplyVector(_direction);
        }

        public void InverseTransform(Matrix4x4 transformMatrix)
        {
            Transform(transformMatrix.inverse);
        }

        public Vector3 GetPoint(float t)
        {
            return _origin + _direction * t;
        }

        /// <summary>
        /// Checks if the ray intersects the specified plane.
        /// </summary>
        /// <param name="t">
        /// At the end of the function call this will hold the intersection offset along the ray 
        /// direction vector. If no intersection occurs, this will be set to 0.0f.
        /// </param>
        /// <returns>
        /// True if the ray intersects the plane and false otherwise. An intersection occurs only 
        /// if the intersection offset is in the [0, 1] range (i.e. intersections along the reverse 
        /// of the ray direction and past the direction length are not allowed).
        /// </returns>
        public bool IntersectsPlane(Plane plane, out float t)
        {
            t = 0.0f;

            // Calculate the distance from the ray origin to the plane and the project the the ray
            // direction vector on the plane normal.
            float originDistanceFromPlane = plane.GetDistanceToPoint(_origin);
            float directionProjectionOnPlaneNormal = Vector3.Dot(plane.normal, _direction);

            // If the projected ray direction is close to 0, it means it runs perpendicualr to the plane 
            // normal and in that case it can not possible intersect the plane.
            if (Mathf.Abs(directionProjectionOnPlaneNormal) < 1e-5f) return false;

            // Calculate the intersection offset. This is the ratio between the ray origin distance
            // from the plane and the ray direction projected on the plane normal. 
            t = -(originDistanceFromPlane / directionProjectionOnPlaneNormal);

            // Only accept values in the [0, 1] interval
            if (t < 0.0f || t > 1.0f) return false;

            // The plane was intersected
            return true;
        }
        #endregion
    }
}
#endif