#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct Circle2D
    {
        #region Private Variables
        private Vector2 _center;
        private float _radius;
        #endregion

        #region Public Properties
        public Vector2 Center { get { return _center; } set { _center = value; } }
        public float Radius { get { return _radius; } set { _radius = value; } }
        #endregion

        #region Constructors
        public Circle2D(Vector2 center, float radius)
        {
            _center = center;
            _radius = radius;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method can be used to check if the specified 2D ray intersects the circle.
        /// </summary>
        /// <param name="ray">
        /// The ray involved in the intersection test.
        /// </param>
        /// <param name="t">
        /// If the ray intersects the circle, this will hold the intersection offset between
        /// the ray and the circle. It will always have a value in the interval [0, 1]. If no
        /// intersection happens, it will be set to 0.
        /// </param>
        /// <returns>
        /// True if an intersection happens and false otherwise.
        /// </returns>
        public bool Raycast(Ray2D ray, out float t)
        {
            t = 0.0f;

            // Calculate the equation coefficients
            Vector2 fromCenterToRayOrigin = ray.Origin - _center;
            float a = Vector3.Dot(ray.Direction, ray.Direction);
            float b = 2.0f * Vector3.Dot(fromCenterToRayOrigin, ray.Direction);
            float c = Vector3.Dot(fromCenterToRayOrigin, fromCenterToRayOrigin) - _radius * _radius;

            // If the equation can be solved, it means that ray intersects the circle
            float t1, t2;
            if (Equation.SolveQuadratic(a, b, c, out t1, out t2))
            {
                // Make sure that the ray does not intersect the circle from behind or that the intersection 
                // offset doesn't exceed the length of the ray direction.
                if (t1 < 0.0f || t1 > 1.0f) return false;

                // Store the intersection offset and return true
                t = t1;
                return true;
            }
            else return false;      // The equation could not be solved, so no intersection occurs
        }

        /// <summary>
        /// Ths method can be used to check if the specified ray intersects the
        /// circle. It returns true if there is an intersection and false otherwise.
        /// </summary>
        public bool Raycast(Ray2D ray)
        {
            float t;
            return Raycast(ray, out t);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return (point - Center).magnitude <= Radius;
        }
        #endregion
    }
}
#endif