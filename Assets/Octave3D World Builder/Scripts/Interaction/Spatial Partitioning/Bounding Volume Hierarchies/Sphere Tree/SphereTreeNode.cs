#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class SphereTreeNode<T>
    {
        #region Private Variables
        private T _data;
        private SphereTree<T> _tree;
        private SphereTreeNode<T> _parent;
        private List<SphereTreeNode<T>> _children = new List<SphereTreeNode<T>>();
        private Sphere _sphere;
        private SphereTreeNodeFlags _flags = SphereTreeNodeFlags.None;
        #endregion

        #region Public Properties
        public SphereTree<T> Tree { get { return _tree; } }
        public T Data { get { return _data; } set { _data = value; } }
        public SphereTreeNode<T> Parent { get { return _parent; } }
        public List<SphereTreeNode<T>> Children { get { return _children; } }
        public int NumberOfChildren { get { return _children.Count; } }
        public Sphere Sphere { get { return _sphere; } set { _sphere = value; } }
        public Vector3 Center { get { return _sphere.Center; } set { _sphere.Center = value; } }
        public float Radius { get { return _sphere.Radius; } set { _sphere.Radius = value; } }
        public SphereTreeNodeFlags Flags { get { return _flags; } }
        public bool IsTerminal { get { return (_flags & SphereTreeNodeFlags.Terminal) != 0; } }
        public bool IsRoot { get { return (_flags & SphereTreeNodeFlags.Root) != 0; } }
        public bool IsSuperSphere { get { return (_flags & SphereTreeNodeFlags.SuperSphere) != 0; } }
        public bool MustRecompute { get { return (_flags & SphereTreeNodeFlags.MustRecompute) != 0; } }
        public bool MustIntegrate { get { return (_flags & SphereTreeNodeFlags.MustIntegrate) != 0; } }
        public bool HasParent { get { return _parent != null; } }
        public bool HasNoChildren { get { return NumberOfChildren == 0; } }
        public bool HasChildren { get { return NumberOfChildren != 0; } }
        #endregion

        #region Constructors
        public SphereTreeNode(Vector3 center, float radius, SphereTree<T> tree, T data = default(T))
        {
            _tree = tree;
            _data = data;
            _sphere.Center = center;
            _sphere.Radius = radius;
        }

        public SphereTreeNode(Sphere sphere, SphereTree<T> tree, T data = default(T))
        {
            _tree = tree;
            _data = data;
            _sphere = sphere;
        }
        #endregion

        #region Public Methods
        public void SetFlag(SphereTreeNodeFlags flag)
        {
            _flags |= flag;
        }

        public void ClearFlag(SphereTreeNodeFlags flag)
        {
            _flags &= ~flag;
        }

        public bool ContainsChild(SphereTreeNode<T> child)
        {
            return _children.Contains(child);
        }

        public bool FullyContains(SphereTreeNode<T> node)
        {
            return _sphere.FullyOverlaps(node.Sphere);
        }

        public void Encapsulate(SphereTreeNode<T> node)
        {
            _sphere = _sphere.Encapsulate(node.Sphere);
        }

        public float GetDistanceBetweenCentersSq(SphereTreeNode<T> node)
        {
            return _sphere.GetDistanceBetweenCentersSq(node.Sphere);
        }

        public void AddChild(SphereTreeNode<T> child)
        {
            if (ContainsChild(child)) return;

            if (child.HasParent) child.Parent.RemoveChild(child);

            _children.Add(child);
            child._parent = this;
        }

        public void RemoveChild(SphereTreeNode<T> child)
        {
            int indexOfChild = _children.FindIndex(item => item == child);
            if (indexOfChild >= 0)
            {
                _children.RemoveAt(indexOfChild);
                child._parent = null;
            }
        }

        public float GetDistanceToNodeExitPoint(SphereTreeNode<T> node)
        {
            return (Center - node.Center).magnitude + node.Radius;
        }

        public void RecomputeCenterAndRadius()
        {
            if(NumberOfChildren != 0)
            {
                // Calculate the new center position of the node. This is the average position of all child nodes.
                Vector3 childPositionSum = Vector3.zero;
                foreach (SphereTreeNode<T> childNode in _children) childPositionSum += childNode.Center;
                Center = childPositionSum / NumberOfChildren;

                // Calculate the sphere radius. This must be large enough to encapsulate all children.
                float maxRadius = float.MinValue;
                foreach(SphereTreeNode<T> childNode in _children)
                {
                    float distanceToChildExitPoint = GetDistanceToNodeExitPoint(childNode);
                    if (distanceToChildExitPoint > maxRadius) maxRadius = distanceToChildExitPoint;
                }
                Radius = maxRadius;

                if (Parent != null && !Parent.MustRecompute) Parent.RecomputeCenterAndRadius();

                // The node was recomputed, so we must clear the recomputation flag
                ClearFlag(SphereTreeNodeFlags.MustRecompute);
            }
        }

        public void EncapsulateChildNode(SphereTreeNode<T> node)
        {
            SphereTreeNode<T> currentParent = this;
            SphereTreeNode<T> currentChild = node;

            while(currentParent != null && !currentParent.FullyContains(currentChild))
            {
                currentParent.Encapsulate(currentChild);
                currentChild = currentParent;
                currentParent = currentParent.Parent;
            }
        }

        public void EncapsulateChildrenBottomUp()
        {
            if (NumberOfChildren != 0)
            {
                SphereTreeNode<T> parent = this;
                while (parent != null)
                {
                    Vector3 centerSum = Vector3.zero;
                    foreach (var child in parent._children) centerSum += child.Center;
                    parent.Center = centerSum * (1.0f / parent.NumberOfChildren);

                    // Now we will calculate the radius which the node must have so that
                    // it can encapsulate all its children.
                    float maxRadius = float.MinValue;
                    foreach (var child in parent._children)
                    {
                        float distToExitPt = (child.Center - parent._sphere.Center).magnitude + child.Radius;
                        if (distToExitPt > maxRadius) maxRadius = distToExitPt;
                    }
                    parent.Radius = maxRadius;
                    parent = parent.Parent;
                }
            }
        }
        #endregion
    }
}
#endif