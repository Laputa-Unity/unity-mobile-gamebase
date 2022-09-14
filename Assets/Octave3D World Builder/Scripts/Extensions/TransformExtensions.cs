#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

namespace O3DWB
{
    public static class TransformExtensions
    {
        #region Extension Methods
        public static TransformMatrix GetWorldMatrix(this Transform transform)
        {
            return new TransformMatrix(transform.position, transform.rotation, transform.lossyScale);
        }

        public static TransformMatrix GetRelativeTransformMatrix(this Transform transform, Transform referenceTransform)
        {
            // Note: Careful! Does not take into account the scale sign. Only works when the sign of the scale is not important.
            Matrix4x4 relativeMatrix = referenceTransform.localToWorldMatrix.inverse * transform.localToWorldMatrix;
            return new TransformMatrix(relativeMatrix);
        }

        public static TransformMatrix GetRelativeTransformMatrix(this Transform transform, TransformMatrix referenceTransform)
        {
            // Note: Careful! Does not take into account the scale sign. Only works when the sign of the scale is not important.
            Matrix4x4 relativeMatrix = referenceTransform.ToMatrix4x4x.inverse * transform.localToWorldMatrix;
            return new TransformMatrix(relativeMatrix);
        }

        public static List<Transform> GetImmediateChildTransforms(this Transform transform)
        {
            var childTransforms = new List<Transform>();
            for (int childIndex = 0; childIndex < transform.childCount; ++childIndex )
            {
                childTransforms.Add(transform.GetChild(childIndex));
            }

            return childTransforms;
        }

        public static void SetWorldScale(this Transform transform, Vector3 worldScale)
        {
            Transform transformParent = transform.parent;
            transform.parent = null;
            transform.localScale = worldScale;
            transform.parent = transformParent;
        }

        public static void InheritWorldTransformFrom(this Transform destinationTransform, Transform sourceTransform)
        {
            destinationTransform.position = sourceTransform.position;
            destinationTransform.rotation = sourceTransform.rotation;
            destinationTransform.localScale = sourceTransform.localScale;
            //destinationTransform.SetWorldScale(sourceTransform.lossyScale);
        }

        public static void InheritLocalTransformFrom(this Transform destinationTransform, Transform sourceTransform)
        {
            destinationTransform.localPosition = sourceTransform.localPosition;
            destinationTransform.localRotation = sourceTransform.localRotation;
            destinationTransform.localScale = sourceTransform.localScale;
        }
        #endregion
    }
}
#endif