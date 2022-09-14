#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct SphereTreeNodeRayHit<T>
    {
        #region Private Variables
        private Ray _ray;
        private SphereTreeNode<T> _hitNode;
        private Vector3 _hitPoint;
        #endregion

        #region Public Properties
        public Ray Ray { get { return _ray; } }
        public SphereTreeNode<T> HitNode { get { return _hitNode; } }
        public Vector3 HitPoint { get { return _hitPoint; } }
        #endregion

        #region Constructors
        public SphereTreeNodeRayHit(Ray ray, float t, SphereTreeNode<T> hitNode)
        {
            _ray = ray;
            _hitNode = hitNode;
            _hitPoint = _ray.GetPoint(t);
        }
        #endregion
    }
}
#endif