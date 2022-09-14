#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class CapsuleColliderExtensions
    {
        public static void CopyDataFrom(this CapsuleCollider collider, CapsuleCollider other)
        {
            collider.center = other.center;
            collider.radius = other.radius;
            collider.height = other.height;
            collider.direction = other.direction;
            collider.isTrigger = other.isTrigger;
            collider.sharedMaterial = other.sharedMaterial;
        }

        public static CapsuleCollider CloneAsNewObject(this CapsuleCollider capsuleCollider, string colliderObjectName)
        {
            if (colliderObjectName == null) return null;

            GameObject originalObject = capsuleCollider.gameObject;
            GameObject cloneObject = new GameObject(colliderObjectName);

            Transform cloneObjectTransform = cloneObject.transform;
            Transform originalObjectTransform = originalObject.transform;
            cloneObjectTransform.InheritWorldTransformFrom(originalObjectTransform);

            CapsuleCollider cloneCollider = cloneObject.AddComponent<CapsuleCollider>();
            cloneCollider.CopyDataFrom(capsuleCollider);

            return cloneCollider;
        }
    }
}
#endif