#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class BoxColliderExtensions
    {
        public static void CopyDataFrom(this BoxCollider collider, BoxCollider other)
        {
            collider.center = other.center;
            collider.size = other.size;
            collider.isTrigger = other.isTrigger;
            collider.sharedMaterial = other.sharedMaterial;
        }

        public static BoxCollider CloneAsNewObject(this BoxCollider boxCollider, string colliderObjectName)
        {
            if (colliderObjectName == null) return null;

            GameObject originalObject = boxCollider.gameObject;
            GameObject cloneObject = new GameObject(colliderObjectName);

            Transform cloneObjectTransform = cloneObject.transform;
            Transform originalObjectTransform = originalObject.transform;
            cloneObjectTransform.InheritWorldTransformFrom(originalObjectTransform);

            BoxCollider cloneCollider = cloneObject.AddComponent<BoxCollider>();
            cloneCollider.CopyDataFrom(boxCollider);

            return cloneCollider;
        }
    }
}
#endif