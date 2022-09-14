#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class SphereColliderExtensions
    {
        public static void CopyDataFrom(this SphereCollider collider, SphereCollider other)
        {
            collider.center = other.center;
            collider.radius = other.radius;
            collider.isTrigger = other.isTrigger;
            collider.sharedMaterial = other.sharedMaterial;
        }

        public static SphereCollider CloneAsNewObject(this SphereCollider sphereCollider, string colliderObjectName)
        {
            if (colliderObjectName == null) return null;

            GameObject originalObject = sphereCollider.gameObject;
            GameObject cloneObject = new GameObject(colliderObjectName);

            Transform cloneObjectTransform = cloneObject.transform;
            Transform originalObjectTransform = originalObject.transform;
            cloneObjectTransform.InheritWorldTransformFrom(originalObjectTransform);

            SphereCollider cloneCollider = cloneObject.AddComponent<SphereCollider>();
            cloneCollider.CopyDataFrom(sphereCollider);

            return cloneCollider;
        }
    }
}
#endif