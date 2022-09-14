#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    /// <summary>
    /// This class implements the functionality for a bounding volume hierarchy made up of spheres.
    /// This is essentially a tree data structure in which each node is a sphere. It will allow us
    /// to perform fast queries similar to those exposed by the Unity 'Physics' API.
    /// </summary>
    /// <remarks>
    /// The reason that we need this functionality is because in order to use Unity's 'Physics' API for
    /// raycasts, overlaps etc, we need to have colliders attached to game objects. This could be solved
    /// in 3 ways: 
    ///     a) force users to use objects whith colliders attached by having the tool automatically attach
    ///        colliders when objects are added to the scene. This is how things were done in the initial
    ///        version of the tool and it is incredibly undesirable;
    ///     b) use dummy objects with colliders attached to them. Each dummy object must have the same position, 
    ///        scale and rotation as the original object. Worked well for a while, but when the Decor Paint Brush 
    ///        mode had to be implemented, object instantiation got incredibly slow. It's also not very desirable
    ///        because for each object that you place in the scene you will actually get 2 instead: the object 
    ///        itself and the dummy. Not a problem for smaller scenes, but could definitely hurt performance for 
    ///        larger scenes;
    ///     c) implement custom API to have access to some of the 'Physics' functionality, but with no collider
    ///        requirements. This class allows us to do that.
    /// </remarks>
    public class SphereTree<T>
    {
        #region Protected Variables
        /// <summary>
        /// The root sphere node.
        /// </summary>
        protected SphereTreeNode<T> _rootNode;

        /// <summary>
        /// The number of children a node is allowed to have. The constructor of the class
        /// has a parameter which allows the client code to configure this value as needed.
        /// </summary>
        protected int _numberOfChildNodesPerNode;

        /// <summary>
        /// When nodes are added to the tree, the tree's hierarchy will not be adjusted on the
        /// spot. Instead, the node will be added to this queue, and when the client code must
        /// call 'PerformPendingUpdates' to integrate any pending nodes in the tree's hierarchy.
        /// This is useful easpecially when using sphere trees to organize the game objects in
        /// the scene. Objects can be instantiated quite often and it is more optimal to postpone
        /// the integration process until the next frame update for example. This list is only used
        /// for terminal nodes.
        /// </summary>
        protected Queue<SphereTreeNode<T>> _terminalNodesPendingIntegration = new Queue<SphereTreeNode<T>>();

        /// <summary>
        /// Same as above but this applies onyl to super sphere nodes that must have their center
        /// and radius recalculated.
        /// </summary>
        protected Queue<SphereTreeNode<T>> _nodesPendingRecomputation = new Queue<SphereTreeNode<T>>();
        #endregion

        public Sphere RootSphere { get { return _rootNode != null ? _rootNode.Sphere : new Sphere(Vector3.zero, 0.0f); } }

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numberOfChildNodesPerNode">
        /// The number of child nodes a node is allowed to have.
        /// </param>
        public SphereTree(int numberOfChildNodesPerNode)
        {
            _numberOfChildNodesPerNode = Mathf.Max(2, numberOfChildNodesPerNode);
            CreateRootNode();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// This method renders all nodes in the tree using Unity's 'Gizmos' API. It can be
        /// used for debugging purposes.
        /// </summary>
        public void RenderGizmosDebug(TransformMatrix transformMatrix)
        {
            // We will use a transparent green color to render the tree nodes. We will also
            // activate the identity matrix because the necessary transform information resides
            // inside the nodes themsleves (the center is all we need actually).
            GizmosColor.Push(new Color(0.0f, 1.0f, 0.0f, 0.3f));
            GizmosMatrix.Push(transformMatrix.ToMatrix4x4x);

            // Start rendering from the root
            RenderGizmosDebugRecurse(_rootNode);

            // Restore the previous color and matrix
            GizmosMatrix.Pop();
            GizmosColor.Pop();
        }

        /// <summary>
        /// Performs a raycast using the specified ray and returns a list of hit information
        /// for each terminal node that was hit.
        /// </summary>
        public List<SphereTreeNodeRayHit<T>> RaycastAll(Ray ray)
        {
            var hitTerminalNodes = new List<SphereTreeNodeRayHit<T>>(20);
            RaycastAllRecurse(ray, _rootNode, hitTerminalNodes);

            return hitTerminalNodes;
        }

        /// <summary>
        /// Returns a list that contains all terminal nodes that intersect or are fully
        /// encapsulated by the specified sphere.
        /// </summary>
        public List<SphereTreeNode<T>> OverlapSphere(Sphere sphere)
        {
            var overlappedTerminalNodes = new List<SphereTreeNode<T>>(20);
            OverlapSphereRecurse(sphere, _rootNode, overlappedTerminalNodes);
            return overlappedTerminalNodes;
        }

        /// <summary>
        /// Returns a list that contains all terminal nodes that intersect or are fully
        /// encapsulated by the specified box.
        /// </summary>
        public List<SphereTreeNode<T>> OverlapBox(OrientedBox box)
        {
            var overlappedTerminalNodes = new List<SphereTreeNode<T>>(20);
            OverlapBoxRecurse(box, _rootNode, overlappedTerminalNodes);
            return overlappedTerminalNodes;
        }

        /// <summary>
        /// Returns a list that contains all terminal nodes that intersect or are fully
        /// encapsulated by the specified box.
        /// </summary>
        public List<SphereTreeNode<T>> OverlapBox(Box box)
        {
            var overlappedTerminalNodes = new List<SphereTreeNode<T>>(20);
            OverlapBoxRecurse(box.ToOrientedBox(), _rootNode, overlappedTerminalNodes);
            return overlappedTerminalNodes;
        }

        /// <summary>
        /// The client code must call this function during each frame update in order
        /// to ensure that any pending node updates are performed.
        /// </summary>
        public void PerformPendingUpdates()
        {
            // First, ensure that all nodes are recomputed accordingly
            while(_nodesPendingRecomputation.Count != 0)
            {
                SphereTreeNode<T> node = _nodesPendingRecomputation.Dequeue();

                // Note: If the node has any children left, we will recompute its volume. Otherwise,
                //       we will remove the node from the tree.
                if (node.HasChildren) node.RecomputeCenterAndRadius();
                else RemoveNode(node);
            }

            // At this point, all super sphere nodes have had their volume updated accordingly.
            // In the next step, we will integrate any necessary terminal nodes.
            while (_terminalNodesPendingIntegration.Count != 0)
            {
                SphereTreeNode<T> node = _terminalNodesPendingIntegration.Dequeue();
                IntegrateTerminalNode(node);
            }
        }

        /// <summary>
        /// Adds a terminal node to the tree.
        /// </summary>
        /// <remarks>
        /// The function does not integrate the node inside the sphere hierarchy. It
        /// will only add it to the integration pending queue. The actual integration
        /// process will be performed inside 'PerformPendingUpdates'.
        /// </remarks>
        /// <param name="sphere">
        /// The node's sphere.
        /// </param>
        /// <param name="data">
        /// The node's data.
        /// </param>
        /// <returns>
        /// The node which was added to the tree.
        /// </returns>
        public SphereTreeNode<T> AddTerminalNode(Sphere sphere, T data)
        {
            // Create a new node and mark it as terminal
            var newTerminalNode = new SphereTreeNode<T>(sphere, this, data);
            newTerminalNode.SetFlag(SphereTreeNodeFlags.Terminal);

            // Add the node to the integration queue
            AddNodeToIntegrationQueue(newTerminalNode);

            return newTerminalNode;
        }

        /// <summary>
        /// Removes the specified node from the tree.
        /// </summary>
        public void RemoveNode(SphereTreeNode<T> node)
        {
            // If the node is not the root node and if it has a parent...
            if(!node.IsRoot && node.Parent != null)
            {
                // Remove the node from its parent
                SphereTreeNode<T> parentNode = node.Parent;
                parentNode.RemoveChild(node);

                // Move up the hierarhcy and remove all parents which don't have any children any more.
                // Note: We will always stop at the root node. The root node is allowed to exist even
                //       if it has no children.
                while (parentNode.Parent != null && parentNode.HasNoChildren && !parentNode.IsRoot)
                {
                    SphereTreeNode<T> newParent = parentNode.Parent;
                    newParent.RemoveChild(parentNode);
                    parentNode = newParent;
                }

                // At this point 'parentNode' references the deepest parent which has at least one child.
                // Because we have removed children from it, its volume must be recalculated, so we add
                // it to the recomputation queue.
                // Note: Even if this function was called from 'PerformPendingUpdates' we still get correct
                //       results because the node will be added to the recomputation queue and it will be 
                //       processed inside the recomputation 'while' loop from where this method is called.
                AddNodeToRecomputationQueue(parentNode);
            }
        }

        /// <summary>
        /// Updates the center of the specified terminal node.
        /// </summary>
        public void UpdateTerminalNodeCenter(SphereTreeNode<T> terminalNode, Vector3 newCenter)
        {
            terminalNode.Center = newCenter;
            OnTerminalNodeSphereUpdated(terminalNode);
        }

        /// <summary>
        /// Updates the radius of the specified terminal node.
        /// </summary>
        public void UpdateTerminalNodeRadius(SphereTreeNode<T> terminalNode, float newRadius)
        {
            terminalNode.Radius = newRadius;
            OnTerminalNodeSphereUpdated(terminalNode);
        }

        /// <summary>
        /// Updates the center and radius of the specified terminal node.
        /// </summary>
        public void UpdateTerminalNodeCenterAndRadius(SphereTreeNode<T> terminalNode, Vector3 newCenter, float newRadius)
        {
            terminalNode.Center = newCenter;
            terminalNode.Radius = newRadius;
            OnTerminalNodeSphereUpdated(terminalNode);
        }

        /// <summary>
        /// The client code can call this method to retrieve a list of serializable
        /// node instances for all nodes inside the tree.
        /// </summary>
        public List<SerializableNodeType> GetSerializableNodes<SerializableNodeType>() where SerializableNodeType : SerializableSphereTreeNode<T>, new()
        {
            // Ensure that the tree is serialized properly by leaving nothing behind (e.g. nodes pending integration or recomputation)
            PerformPendingUpdates();

            var serializableNodes = new List<SerializableNodeType>(200);
            AcquireSerializableNodeRecurse(_rootNode, -1, serializableNodes);
            return serializableNodes;
        }

        /// <summary>
        /// Creates the tree from the specified collection of serialized nodes.
        /// </summary>
        public void CreateTreeFromSerializedNodes<SerializableNodeType>(List<SerializableNodeType> serializedNodes) where SerializableNodeType : SerializableSphereTreeNode<T>, new()
        {
            // If no nodes are present, we just create the root node. Otherwise, we call 'CreateTreeNodesFromSerializedNodes'.
            if (serializedNodes.Count == 0) CreateRootNode();
            else _rootNode = CreateTreeNodesFromSerializedNodes(serializedNodes);
        }

        /// <summary>
        /// Returns a dictionary which maps each terminal node data to the actual terminal node.
        /// </summary>
        public Dictionary<T, SphereTreeNode<T>> GetDataToTerminalNodeDictionary()
        {
            var dictionary = new Dictionary<T, SphereTreeNode<T>>();
            GetDataToTerminalNodeDictionaryRecurse(_rootNode, dictionary);
            return dictionary;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Creates the root node of the tree.
        /// </summary>
        private void CreateRootNode()
        {
            // Create the root node with some sensible default values. It doesn't really matter what
            // initial size the root node has. It will grow as needed when new nodes are added to the
            // tree.
            _rootNode = new SphereTreeNode<T>(Vector3.zero, 10.0f, this);
            _rootNode.SetFlag(SphereTreeNodeFlags.Root | SphereTreeNodeFlags.SuperSphere);
        }

        /// <summary>
        /// Integrates the specified node into the tree hierarchy. Integrating a nod means placing it
        /// inside the tree hierarchy in the correct spot. Because it is a terminal node, the method
        /// will have to search for the correct super-sphere node which can act as a parent of this node.
        /// </summary>
        private void IntegrateTerminalNode(SphereTreeNode<T> nodeToIntegrate)
        {
            // Start a recursive process from the root of the hierarchy. After the integration
            // is finished, we will clear the node's integration flag because it no loger needs
            // to be integrated.
            IntegrateTerminalNodeRecurse(nodeToIntegrate, _rootNode);
            nodeToIntegrate.ClearFlag(SphereTreeNodeFlags.MustIntegrate);
        }

        /// <summary>
        /// This is a recursive method which is responsible for integration the specified
        /// node inside the tree.
        /// </summary>
        private void IntegrateTerminalNodeRecurse(SphereTreeNode<T> nodeToIntegrate, SphereTreeNode<T> parentNode)
        {
            // If this node still has room for children, we will add the integration node here. This 'if' statement
            // will also handle the special case in which only the root node currently exists inside the tree.
            if(parentNode.NumberOfChildren < _numberOfChildNodesPerNode)
            {
                // Add the node as a child of the parent node and ensure that the root node encapsulates it
                parentNode.AddChild(nodeToIntegrate);
                parentNode.EncapsulateChildNode(nodeToIntegrate);
            }
            else
            {
                // If there is no more room, we will proceed by choosing one of the parent's children which
                // is closest to the node that we want to integrate. We choose the closest node because when
                // the node will be added as a child of it, we want the parent to grow as little as possible.
                List<SphereTreeNode<T>> children = parentNode.Children;
                SphereTreeNode<T> closestChild = FindClosestNode(children, nodeToIntegrate);
                if (closestChild == null) return;

                // If the closest child is not a terminal node, recurse.
                if (!closestChild.IsTerminal) IntegrateTerminalNodeRecurse(nodeToIntegrate, closestChild);
                else
                {
                    // If we reached a terminal node, we create a new node which encapsulates 'closestChild' and 'nodeToIntegrate'
                    // and then we replace 'closestChild' with this new node. The first step is to create the sphere of this
                    // new node. It doesn't matter how big this sphere is initially because we will call 'EncapsulateChildNode'
                    // later.
                    Sphere newNodeSphere = closestChild.Sphere;

                    // Create the node using the sphere we just calculated
                    SphereTreeNode<T> newNode = new SphereTreeNode<T>(newNodeSphere, this, default(T));
                    newNode.SetFlag(SphereTreeNodeFlags.SuperSphere);

                    // Replace 'closestChild' with the new node and add both 'closestChild' and 'nodeToIntegrate' as children of it
                    parentNode.RemoveChild(closestChild);
                    parentNode.AddChild(newNode);
                    newNode.AddChild(nodeToIntegrate);
                    newNode.AddChild(closestChild);
                    newNode.EncapsulateChildrenBottomUp();

                    // Encapsulate the children inside the new node
                    //newNode.EncapsulateChildNode(closestChild);
                    //newNode.EncapsulateChildNode(nodeToIntegrate);

                    // Ensure that the new node is fully contained inside the parent node
                    //parentNode.EncapsulateChildNode(newNode);
                }
            }
        }

        /// <summary>
        /// Finds and returns the node inside 'nodes' which is closest to 'node'.
        /// </summary>
        private SphereTreeNode<T> FindClosestNode(List<SphereTreeNode<T>> nodes, SphereTreeNode<T> node)
        {
            float minDistanceSq = float.MaxValue;
            SphereTreeNode<T> closestNode = null;

            // We will choose the node whose center is closest to 'node'
            foreach(SphereTreeNode<T> potentialNode in nodes)
            {
                // Calculate the squared distance between the node centers
                float distanceBetweenNodesSq = potentialNode.GetDistanceBetweenCentersSq(node);
                if(distanceBetweenNodesSq < minDistanceSq)
                {
                    // Smaller than what we have so far?
                    minDistanceSq = distanceBetweenNodesSq;
                    closestNode = potentialNode;

                    // If we somehow managed to find a node which has the same position as 'node', we can exit 
                    if (minDistanceSq == 0.0f) return closestNode;
                }
            }

            return closestNode;
        }

        /// <summary>
        /// Adds the specified node to the recomputation queue.
        /// </summary>
        private void AddNodeToRecomputationQueue(SphereTreeNode<T> node)
        {
            // Only non-terminal, non-root nodes are allowed. We also have to ensure that
            // the node hasn't already been added to the recomputation queue.
            if (node.IsTerminal || node.IsRoot || node.MustRecompute) return;
            if (node.IsSuperSphere)
            {
                node.SetFlag(SphereTreeNodeFlags.MustRecompute);
                _nodesPendingRecomputation.Enqueue(node);
            }
        }

        /// <summary>
        /// Adds the specified node to the integration queue.
        /// </summary>
        private void AddNodeToIntegrationQueue(SphereTreeNode<T> node)
        {
            // Only terminal, non-root nodes are allowed. We also have to ensure that
            // the node hasn't already been added to the integration queue.
            if (node.IsSuperSphere || node.IsRoot || node.MustIntegrate) return;
            if (node.IsTerminal)
            {
                node.SetFlag(SphereTreeNodeFlags.MustIntegrate);
                _terminalNodesPendingIntegration.Enqueue(node);
            }
        }

        /// <summary>
        /// This method is called when the sphere of a terminal node has been updated.
        /// </summary>
        private void OnTerminalNodeSphereUpdated(SphereTreeNode<T> terminalNode)
        {
            // If the node is already marked for reintegration, there is nothing left for us to do
            if (terminalNode.MustIntegrate) return;

            // If the node is now outside of its parent, it may now be closer to another parent and associating
            // it with that new parent may provide better space/volume savings. So we remove the node from its
            // parent and add it to the integration queue so that it can be reintegrated. During the integration
            // process, the algorithm may find a more optimal parent or the same one if a more optimal one doesn't
            // exist.
            // Note: We are only removing the child from its parent if it went outside of its parent volume. It may
            //       probably be a better idea to always check the surrounding parents and see if a more optimal one
            //       exists even if the node doesn't pierce its parent's skin. For the moment however, we will only
            //       remove the child from its parent if it pierced its skin.
            SphereTreeNode<T> parentNode = terminalNode.Parent;
            float distanceToParentNodeExitPoint = parentNode.GetDistanceToNodeExitPoint(terminalNode);
            if (distanceToParentNodeExitPoint > parentNode.Radius)
            {
                // Note: It may be a good idea to check if the node contains only one child after removal. In that
                //       case the node itself can be removed and its child moved up the hierarchy. However, for the
                //       moment we'll keep things simple.
                // Remove the child from its parent and add it to the integration queue. Adding it to
                // the integration queue is necessary in order to ensure that it gets reassigned to
                // the correct parrent based on its current position.
                parentNode.RemoveChild(terminalNode);
                AddNodeToIntegrationQueue(terminalNode);
            }

            // Whenever a terminal node is updated, it's parent must gave its volume recomputed. We always do this
            // regardless of whether or not the node was removed from the parent or not.
            AddNodeToRecomputationQueue(parentNode);
        }

        /// <summary>
        /// Recursive function which renders the specified node in the scene view and then
        /// steps down the hierarchy for each child of the node.
        /// </summary>
        private void RenderGizmosDebugRecurse(SphereTreeNode<T> node)
        {
            // Draw the sphere for the specified node and then recurse for each child node
            Gizmos.DrawSphere(node.Sphere.Center, node.Sphere.Radius);
            foreach (var child in node.Children) RenderGizmosDebugRecurse(child);
        }

        /// <summary>
        /// Recursive method which is used to detect which terminal nodes in the tree are intersected
        /// by 'ray'. Information about the intersected nodes is stored inside 'terminalNodeHitInfo'.
        /// </summary>
        private void RaycastAllRecurse(Ray ray, SphereTreeNode<T> parentNode, List<SphereTreeNodeRayHit<T>> terminalNodeHitInfo)
        {
            // If the parent node is not hit by the ray, there is no need to go further
            float t;
            if (!parentNode.Sphere.Raycast(ray, out t)) return;
            else
            {
                // If the parent node was hit, we have 2 choices:
                //  a) if the node is a terminal node, we add it to the 'hitNodes' list and exit;
                //  b) if the node is a super-sphere, we will recurse for eacoh if its children.
                if(parentNode.IsTerminal)
                {
                    terminalNodeHitInfo.Add(new SphereTreeNodeRayHit<T>(ray, t, parentNode));
                    return;
                }
                else
                {
                    // Recurse for each child node
                    List<SphereTreeNode<T>> childNodes = parentNode.Children;
                    foreach (SphereTreeNode<T> childNode in childNodes) RaycastAllRecurse(ray, childNode, terminalNodeHitInfo);
                }
            }
        }

        /// <summary>
        /// Recursive method which is used to step down the tree hierarchy collecting all terminal nodes
        /// which are overlapped by the specified sphere.
        /// </summary>
        private void OverlapSphereRecurse(Sphere sphere, SphereTreeNode<T> parentNode, List<SphereTreeNode<T>> overlappedTerminalNodes)
        {
            // If the parent is not overlapped there is no need to go any further
            if (!parentNode.Sphere.OverlapsFullyOrPartially(sphere)) return;
            else
            {
                // If this is a terminal node, add it to the output list and return
                if(parentNode.IsTerminal)
                {
                    overlappedTerminalNodes.Add(parentNode);
                    return;
                }
                else
                {
                    // Recurs for each child node
                    List<SphereTreeNode<T>> childNodes = parentNode.Children;
                    foreach (SphereTreeNode<T> childNode in childNodes) OverlapSphereRecurse(sphere, childNode, overlappedTerminalNodes);
                }
            }
        }

        /// <summary>
        /// Recursive method which is used to step down the tree hierarchy collecting all terminal nodes
        /// which are overlapped by the specified box.
        /// </summary>
        private void OverlapBoxRecurse(OrientedBox box, SphereTreeNode<T> parentNode, List<SphereTreeNode<T>> overlappedTerminalNodes)
        {
            // If the parent is not overlapped there is no need to go any further
            if (!parentNode.Sphere.OverlapsFullyOrPartially(box)) return;
            else
            {
                // If this is a terminal node, add it to the output list and return
                if (parentNode.IsTerminal)
                {
                    overlappedTerminalNodes.Add(parentNode);
                    return;
                }
                else
                {
                    // Recurs for each child node
                    List<SphereTreeNode<T>> childNodes = parentNode.Children;
                    foreach (SphereTreeNode<T> childNode in childNodes) OverlapBoxRecurse(box, childNode, overlappedTerminalNodes);
                }
            }
        }

        /// <summary>
        /// This method is called in order to retrieve an instance of a serializable node type.
        /// The serializable node instance is added to the 'serializableNodes' list. This is a 
        /// recursive method which steps down the entire hierarchy.
        /// </summary>
        private void AcquireSerializableNodeRecurse<SerializableNodeType>(SphereTreeNode<T> parentNode, int parentNodeIndex, List<SerializableNodeType> serializedNodes) where SerializableNodeType : SerializableSphereTreeNode<T>, new()
        {
            // Create a serializable node instance for the current node
            SerializableNodeType serializableNode = new SerializableNodeType();
            serializableNode.NodeData = parentNode.Data;
            serializableNode.SphereCenter = parentNode.Center;
            serializableNode.SphereRadius = parentNode.Radius;
            serializableNode.ParentNodeIndex = parentNodeIndex;
            serializableNode.Flags = parentNode.Flags;
            serializedNodes.Add(serializableNode);

            // Recurse for each child
            int newParentNodeIndex = serializedNodes.Count - 1;
            foreach (var childNode in parentNode.Children) AcquireSerializableNodeRecurse(childNode, newParentNodeIndex, serializedNodes);
        }

        /// <summary>
        /// Creates the tree nodes from the specified list of serialized nodes.
        /// </summary>
        /// <returns>
        /// The tree root node.
        /// </returns>
        private SphereTreeNode<T> CreateTreeNodesFromSerializedNodes<SerializableNodeType>(List<SerializableNodeType> serializedNodes) where SerializableNodeType : SerializableSphereTreeNode<T>, new()
        {
            var allNodes = new List<SphereTreeNode<T>>();
            foreach(var serializedNode in serializedNodes)
            {
                // Create the actual node instance and store it in the node list
                var node = new SphereTreeNode<T>(new Sphere(serializedNode.SphereCenter, serializedNode.SphereRadius), this, serializedNode.NodeData);
                node.SetFlag(serializedNode.Flags);
                allNodes.Add(node);

                // If the node has a parent, establish the parent-child relationship
                if (serializedNode.ParentNodeIndex >= 0)
                {
                    SphereTreeNode<T> parentNode = allNodes[serializedNode.ParentNodeIndex];
                    parentNode.AddChild(node);
                }               
            }

            // Return the root node
            return allNodes[0];
        }

        /// <summary>
        /// Recursive method which steps down the tree hierarchy and adds pairs of node-data/node to 'dictionary'.
        /// When this method returns, all the terminal nodes and their data will be stored inside 'dictionary'.
        /// </summary>
        private void GetDataToTerminalNodeDictionaryRecurse(SphereTreeNode<T> parentNode, Dictionary<T, SphereTreeNode<T>> dictionary)
        {
            if (parentNode.IsTerminal) dictionary.Add(parentNode.Data, parentNode);
            else
            {
                List<SphereTreeNode<T>> children = parentNode.Children;
                foreach (var child in children) GetDataToTerminalNodeDictionaryRecurse(child, dictionary);
            }
        }
        #endregion
    }
}
#endif