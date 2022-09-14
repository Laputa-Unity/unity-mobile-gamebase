#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class MeshColliderExtensions
    {
        public static void CopyDataFrom(this MeshCollider collider, MeshCollider other)
        {
            collider.convex = other.convex;
            collider.sharedMesh = other.sharedMesh;
            collider.isTrigger = other.isTrigger;
            collider.sharedMaterial = other.sharedMaterial;
        }

        public static MeshCollider CloneAsNewObject(this MeshCollider meshCollider, string colliderObjectName)
        {
            if (colliderObjectName == null) return null;

            GameObject originalObject = meshCollider.gameObject;
            GameObject cloneObject = new GameObject(colliderObjectName);

            Transform cloneObjectTransform = cloneObject.transform;
            Transform originalObjectTransform = originalObject.transform;
            cloneObjectTransform.InheritWorldTransformFrom(originalObjectTransform);

            MeshCollider cloneCollider = cloneObject.AddComponent<MeshCollider>();
            cloneCollider.CopyDataFrom(meshCollider);

            return cloneCollider;
        }
    }
}
#endif