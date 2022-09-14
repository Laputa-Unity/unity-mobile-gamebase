using UnityEngine;

namespace O3DWB
{
    public static class ObjectHierarchySnap
    {
        public static void Snap(GameObject root, Vector3 pivotPoint, Vector3 destPoint)
        {
            Transform rootTransform = root.transform;
            Vector3 snapVector = rootTransform.position - pivotPoint;
            rootTransform.position = destPoint + snapVector;
        }
    }
}
