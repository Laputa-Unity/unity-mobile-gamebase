#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class Matrix4x4Extensions
    {
        #region Extension Methods
        public static Vector3 GetTranslation(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        public static Matrix4x4 SetTranslation(this Matrix4x4 matrix, Vector3 translation)
        {
            matrix.SetColumn(3, new Vector4(translation.x, translation.y, translation.z, 1.0f));
            return matrix;
        }

        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            Vector3 upAxis = matrix.GetNormalizedUpAxis();
            Vector3 lookAxis = matrix.GetNormalizedLookAxis();

            return Quaternion.LookRotation(lookAxis, upAxis);
        }
        
        public static Matrix4x4 SetRotation(this Matrix4x4 matrix, Quaternion rotation)
        {
            Vector3 translation = matrix.GetTranslation();
            Vector3 scale = matrix.GetXYZScale();

            return Matrix4x4.TRS(translation, rotation, scale);
        }

        // Note: Returns only positive scale values even if the matrix contains negative scale.
        public static Vector3 GetXYZScale(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.GetColumn(0).magnitude, 
                               matrix.GetColumn(1).magnitude, 
                               matrix.GetColumn(2).magnitude);
        }

        // Note: Works only with scale matrices (no rotation data).
        public static Vector3 GetSignedXYZScale(this Matrix4x4 matrix)
        {
            return new Vector3(matrix.GetColumn(0)[0], matrix.GetColumn(1)[1], matrix.GetColumn(2)[2]);
        }

        public static Matrix4x4 SetXYZScale(this Matrix4x4 matrix, Vector3 scale)
        {
            Vector3 translation = matrix.GetTranslation();
            Quaternion rotation = matrix.GetRotation();

            return Matrix4x4.TRS(translation, rotation, scale);
        }

        public static Matrix4x4 SetXYZScale(this Matrix4x4 matrix, float scale)
        {
            return matrix.SetXYZScale(new Vector3(scale, scale, scale));
        }

        public static Vector3 GetNormalizedRightAxis(this Matrix4x4 matrix)
        {
            Vector3 rightAxis = matrix.GetColumn(0);
            rightAxis.Normalize();

            return rightAxis;
        }

        public static Vector3 GetNormalizedUpAxis(this Matrix4x4 matrix)
        {
            Vector3 upAxis = matrix.GetColumn(1);
            upAxis.Normalize();

            return upAxis;
        }

        public static Vector3 GetNormalizedLookAxis(this Matrix4x4 matrix)
        {
            Vector3 lookAxis = matrix.GetColumn(2);
            lookAxis.Normalize();

            return lookAxis;
        }

        public static Vector3 GetNormalizedAxis(this Matrix4x4 matrix, int axisIndex)
        {
            Vector3 normalizedAxis = matrix.GetColumn(axisIndex);
            normalizedAxis.Normalize();

            return normalizedAxis;
        }

        public static Vector3[] GetNormalizedLocalAxes(this Matrix4x4 matrix)
        {
            Vector3[] localAxes = new Vector3[3];
            localAxes[0] = matrix.GetNormalizedRightAxis();
            localAxes[1] = matrix.GetNormalizedUpAxis();
            localAxes[2] = matrix.GetNormalizedLookAxis();

            return localAxes;
        } 

        public static int GetIndexOfAxisMostAlignedWith(this Matrix4x4 matrix, Vector3 referenceAxis)          
        {
            int bestAxisIndex = -1;
            float bestAbsDotProduct = -1.0f;
            referenceAxis.Normalize();

            for(int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                Vector3 axis = matrix.GetNormalizedAxis(axisIndex);

                if(axis.IsAlignedWith(referenceAxis)) return axisIndex;
                else
                {
                    float absDotProduct = axis.GetAbsDot(referenceAxis);
                    if (absDotProduct > bestAbsDotProduct)
                    {
                        bestAbsDotProduct = absDotProduct;
                        bestAxisIndex = axisIndex;
                    }
                }
            }

            return bestAxisIndex;
        }

        public static int GetIndexOfAxisAlignedWith(this Matrix4x4 matrix, Vector3 referenceAxis)
        {
            referenceAxis.Normalize();
            for (int axisIndex = 0; axisIndex < 3; ++axisIndex)
            {
                Vector3 axis = matrix.GetNormalizedAxis(axisIndex);
                if (axis.IsAlignedWith(referenceAxis)) return axisIndex;
            }

            return -1;
        }
        #endregion
    }
}
#endif