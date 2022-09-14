#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectPositionCalculator
    {
        #region Public Static Functions
        public static Vector3 CalculateObjectHierarchyPosition(Prefab sourcePrefab, Vector3 desiredOrientedBoxCenter, Vector3 desiredWorldScale, Quaternion desiredWorldRotation)
        {
            return CalculateObjectHierarchyPosition(sourcePrefab.UnityPrefab, desiredOrientedBoxCenter, desiredWorldScale, desiredWorldRotation);
        }

        public static Vector3 CalculateObjectHierarchyPosition(GameObject source, Vector3 desiredOrientedBoxCenter, Vector3 desiredWorldScale, Quaternion desiredWorldRotation)
        {
            OrientedBox prefabWorldOrientedBox = source.GetHierarchyWorldOrientedBox();
            Transform rootTransform = source.transform;

            Matrix4x4 rootTransformMatrix = Matrix4x4.TRS(Vector3.zero, rootTransform.rotation, rootTransform.lossyScale);
            Matrix4x4 desiredTransformMatrix = Matrix4x4.TRS(Vector3.zero, desiredWorldRotation, desiredWorldScale);
            Matrix4x4 inverseTransformMatrix = desiredTransformMatrix * rootTransformMatrix.inverse;

            Vector3 relationshipVector = rootTransform.position - prefabWorldOrientedBox.Center;
            relationshipVector = inverseTransformMatrix.MultiplyVector(relationshipVector);

            return desiredOrientedBoxCenter + relationshipVector;
        }
        #endregion
    }
}
#endif