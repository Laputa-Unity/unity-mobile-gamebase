#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class Equation
    {
        #region Public Static Functions
        /// <summary>
        /// Solves a quadratic equation with the specified coefficients.
        /// </summary>
        /// <param name="a">
        /// The 'a' coefficient of the quadratic equation.
        /// </param>
        /// <param name="b">
        /// The 'b' coefficient of the quadratic equation.
        /// </param>
        /// <param name="c">
        /// The 'x' coefficient of the quadratic equation.
        /// </param>
        /// <param name="t1">
        /// If the equation can be solved, this will hold the solution with the smallest value. If
        /// the equation has only one solution, this will be the same as t2. If no solutions can be
        /// found (i.e. equation can not be solved) this will be set to 0.
        /// </param>
        /// <param name="t2">
        /// If the equation can be solved, this will hold the solution with the biggest value. If
        /// the equation has only one solution, this will be the same as t1. If no solutions can be
        /// found (i.e. equation can not be solved) this will be set to 0.
        /// </param>
        /// <returns>
        /// True if the equation can be solved and false otherwise.
        /// </returns>
        public static bool SolveQuadratic(float a, float b, float c, out float t1, out float t2)
        {
            t1 = t2 = 0.0f;

            // Calculate delta. If less than 0, no solutions exist.
            float delta = b * b - 4.0f * a * c;
            if (delta < 0.0f) return false;

            // We have at least one solution, so cache these for later use
            float deltaSqrt = Mathf.Sqrt(delta);
            float oneOver2a = 1.0f / (2.0f * a);

            // 2 solutions?
            if (delta > 0.0f)
            {
                // Calculate the solutions
                t1 = (-b + deltaSqrt) * oneOver2a;
                t2 = (-b - deltaSqrt) * oneOver2a;
            }
            else t1 = t2 = -b * oneOver2a;      // Only one solution

            // Reorder t values to make sure that t1 is always smaller or equal to t2
            if (t1 > t2)
            {
                float temp = t1;
                t1 = t2;
                t2 = temp;
            }

            // At least one solution was found
            return true;
        }
        #endregion
    }
}
#endif