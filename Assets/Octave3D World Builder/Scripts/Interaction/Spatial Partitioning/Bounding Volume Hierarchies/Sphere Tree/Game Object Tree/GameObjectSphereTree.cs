#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    /// <summary>
    /// Serializble sphere tree node class which is used to serialize the game object tree.
    /// </summary>
    [Serializable]
    public class SerializableGameObjectSphereTreeNode : SerializableSphereTreeNode<GameObject> { }

    /// <summary>
    /// A game object sphere tree class which can be used to speed up different kinds of scene
    /// queries such as raycasts and overlaps.
    /// </summary>
    [Serializable]
    public class GameObjectSphereTree : ISerializationCallbackReceiver
    {
        #region Private Variables
        private int _numberOfChildNodesPerNode;

        /// <summary>
        /// The actual sphere tree. This is the tree that will store the game objects in its
        /// terminal nodes. 
        /// </summary>
        [NonSerialized]
        private SphereTree<GameObject> _sphereTree;

        /// <summary>
        /// Maps each object registered with the tree to the terminal tree node in which it resides.
        /// We will need this information when the transform data of an object has changed. It will
        /// help us access the terminal node which needs to be update given a speciffic object.
        /// </summary>
        /// <remarks>
        /// This is probably the best workaround that I can come up with (at least for the moment).
        /// Normally, the game objects themselves would probably have a reference to the node in
        /// which they reside. However, we can not modify the 'GameObject' class to include this info,
        /// so this is the next best thing.
        /// </remarks>
        [NonSerialized]
        private Dictionary<GameObject, SphereTreeNode<GameObject>> _gameObjectToNode = new Dictionary<GameObject, SphereTreeNode<GameObject>>();

        /// <summary>
        /// We will need this list of serialized nodes to serialize/deserialize the tree accordingly.
        /// </summary>
        [NonSerialized]
        private List<SerializableGameObjectSphereTreeNode> _serializedNodes = new List<SerializableGameObjectSphereTreeNode>();
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the number of game objects which were registered with the tree.
        /// </summary>
        public int NumberOfGameObjects { get { return _gameObjectToNode.Count; } }

        public Sphere RootSphere { get { return _sphereTree.RootSphere; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="numberOfChildNodesPerNode">
        /// The number of child nodes a tree node is allowed to have.
        /// </param>
        public GameObjectSphereTree(int numberOfChildNodesPerNode)
        {
            _numberOfChildNodesPerNode = numberOfChildNodesPerNode;
            _sphereTree = new SphereTree<GameObject>(numberOfChildNodesPerNode);

            EditorApplication.hierarchyChanged -= HierarchyWindowChanged;
            EditorApplication.hierarchyChanged += HierarchyWindowChanged;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Called before the tree is about to serialize.
        /// </summary>
        public void OnBeforeSerialize()
        {
            // Acquire the list of nodes which can be serialized. After the serialization process
            // is completed, this list will be serialized and it can then be used for deserialization.
            //_serializedNodes = _sphereTree.GetSerializableNodes<SerializableGameObjectSphereTreeNode>();
        }

        /// <summary>
        /// Called after the tree has been deserialized.
        /// </summary>
        public void OnAfterDeserialize()
        {
            // Use the list of serializable nodes to construct the sphere tree and then
            // instruct it to give us the dictionary which maps each game object to its
            // terminal node.
            //_sphereTree.CreateTreeFromSerializedNodes(_serializedNodes);
            //_gameObjectToNode = _sphereTree.GetDataToTerminalNodeDictionary();
        }

        /// <summary>
        /// This method can be called to render any necessary gizmos for debugging purposes.
        /// </summary>
        public void RenderGizmosDebug()
        {
            _sphereTree.RenderGizmosDebug(TransformMatrix.GetIdentity());
        }

        /// <summary>
        /// Returns a list of all objects which are overlapped by the specified sphere.
        /// </summary>
        /// <param name="sphere">
        /// The sphere involved in the overlap query.
        /// </param>
        public List<GameObject> OverlapSphere(Sphere sphere)
        {
            // Retrieve all the sphere tree nodes which are overlapped by the sphere. If no nodes are overlapped,
            // we can return an empty list because it means that no objects could possibly be overlapped either.
            List<SphereTreeNode<GameObject>> allOverlappedNodes = _sphereTree.OverlapSphere(sphere);
            if (allOverlappedNodes.Count == 0) return new List<GameObject>();

            // Loop through all overlapped nodes
            var overlappedObjects = new List<GameObject>();
            foreach(SphereTreeNode<GameObject> node in allOverlappedNodes)
            {
                // Store the node's object for easy access
                GameObject gameObject = node.Data;

                // Note: It is important to check for null because the object may have been destroyed. 'RemoveNullObjectNodes'
                //       removes null objects but given the order in which Unity calls certain key functions such as 'OnSceneGUI'
                //       and any editor registered callbacks, null object references can still pop up.
                if (gameObject == null) continue;
                if (!gameObject.activeSelf) continue;

                // We need to perform an additional check. Even though the sphere overlaps the object's node (which is
                // another sphere), we must also check if the sphere overlaps the object's world oriented box. This allows
                // for better precision.
                OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
                if(sphere.OverlapsFullyOrPartially(objectWorldOrientedBox)) overlappedObjects.Add(gameObject);
            }

            return overlappedObjects;
        }

        /// <summary>
        /// Returns a list of all objects which are overlapped by the specified box.
        /// </summary>
        /// <param name="box">
        /// The box involved in the overlap query.
        /// </param>
        public List<GameObject> OverlapBox(OrientedBox box)
        {
            // Retrieve all the sphere tree nodes which are overlapped by the box. If no nodes are overlapped,
            // we can return an empty list because it means that no objects could possibly be overlapped either.
            List<SphereTreeNode<GameObject>> allOverlappedNodes = _sphereTree.OverlapBox(box);
            if (allOverlappedNodes.Count == 0) return new List<GameObject>();

            // Loop through all overlapped nodes
            var overlappedObjects = new List<GameObject>();
            foreach (SphereTreeNode<GameObject> node in allOverlappedNodes)
            {
                // Store the node's object for easy access
                GameObject gameObject = node.Data;
                if (gameObject == null) continue;
                if (!gameObject.activeSelf) continue;

                // We need to perform an additional check. Even though the box overlaps the object's node (which is
                // a sphere), we must also check if the box overlaps the object's world oriented box. This allows
                // for better precision.
                OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
                if (box.Intersects(objectWorldOrientedBox)) overlappedObjects.Add(gameObject);
            }

            return overlappedObjects;
        }

        public bool BoxIntersectsAnyObjectBoxes(OrientedBox box, HashSet<GameObject> ignoreObjects, bool allowFaceTouch)
        {
            if (ignoreObjects == null) ignoreObjects = new HashSet<GameObject>();
            List<SphereTreeNode<GameObject>> allOverlappedNodes = _sphereTree.OverlapBox(box);
            if (allOverlappedNodes.Count == 0) return false;

            foreach (SphereTreeNode<GameObject> node in allOverlappedNodes)
            {
                GameObject gameObject = node.Data;
                if (gameObject == null) continue;
                if (!gameObject.activeSelf) continue;
                if (ignoreObjects.Contains(gameObject)) continue;

                OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
                if (box.Intersects(objectWorldOrientedBox))
                {
                    if (!allowFaceTouch) return true;
                    else
                    {
                        if (!box.AreAllBoxPointsOnOrInFrontOfAnyFacePlane(objectWorldOrientedBox)) return true;
                    }
                }
            }

            return false;
        }

        public bool ObjectMeshIntersectsAnyMesh(Octave3DMesh octave3DQueryMesh, TransformMatrix worldMatrix, HashSet<GameObject> ignoreObjects)
        {
            if (ignoreObjects == null) ignoreObjects = new HashSet<GameObject>();
            OrientedBox queryMeshOOBB = octave3DQueryMesh.GetOOBB(worldMatrix);
            if (queryMeshOOBB.IsInvalid()) return false;

            List<SphereTreeNode<GameObject>> allOverlappedNodes = _sphereTree.OverlapBox(queryMeshOOBB);
            if (allOverlappedNodes.Count == 0) return false;

            foreach (SphereTreeNode<GameObject> node in allOverlappedNodes)
            {
                GameObject gameObject = node.Data;
                if (gameObject == null) continue;
                if (!gameObject.activeSelf) continue;
                if (ignoreObjects.Contains(gameObject)) continue;

                Mesh objectMesh = gameObject.GetMeshFromFilterOrSkinnedMeshRenderer();
                if (objectMesh != null)
                {
                    Octave3DMesh octave3DMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(objectMesh);
                    if (octave3DMesh != null)
                    {
                        if (octave3DQueryMesh.IntersectsMesh(worldMatrix, octave3DMesh, gameObject.transform.GetWorldMatrix())) return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a list of all objects which are overlapped by the specified box.
        /// </summary>
        /// <param name="box">
        /// The box involved in the overlap query.
        /// </param>
        public List<GameObject> OverlapBox(Box box)
        {
            // Retrieve all the sphere tree nodes which are overlapped by the box. If no nodes are overlapped,
            // we can return an empty list because it means that no objects could possibly be overlapped either.
            List<SphereTreeNode<GameObject>> allOverlappedNodes = _sphereTree.OverlapBox(box);
            if (allOverlappedNodes.Count == 0) return new List<GameObject>();

            // Loop through all overlapped nodes
            OrientedBox orientedBox = box.ToOrientedBox();
            var overlappedObjects = new List<GameObject>();
            foreach (SphereTreeNode<GameObject> node in allOverlappedNodes)
            {
                // Store the node's object for easy access
                GameObject gameObject = node.Data;
                if (gameObject == null) continue;
                if (!gameObject.activeSelf) continue;

                // We need to perform an additional check. Even though the box overlaps the object's node (which is
                // a sphere), we must also check if the box overlaps the object's world oriented box. This allows
                // for better precision.
                OrientedBox objectWorldOrientedBox = gameObject.GetWorldOrientedBox();
                if (orientedBox.Intersects(objectWorldOrientedBox)) overlappedObjects.Add(gameObject);
            }
 
            return overlappedObjects;
        }

        /// <summary>
        /// Performs a raycast and returns a list of hits for all objects whose oriented 
        /// boxes are intersected by the specified ray.
        /// </summary>
        public List<GameObjectRayHit> RaycastAllBox(Ray ray)
        {
            // First, retrieve a list of the sphere tree nodes which were hit by the ray.
            // If no nodes were hit, it means no object was hit either.
            List<SphereTreeNodeRayHit<GameObject>> allNodeHits = _sphereTree.RaycastAll(ray);
            if (allNodeHits.Count == 0) return new List<GameObjectRayHit>();

            // Loop through all nodes which were hit by the ray. For each node, we have to detect
            // if the ray hits the actual object box.
            var gameObjectHits = new List<GameObjectRayHit>();
            foreach(SphereTreeNodeRayHit<GameObject> nodeHit in allNodeHits)
            {
                // Retrieve the object which resides in the node
                GameObject gameObject = nodeHit.HitNode.Data;
                if (gameObject == null) continue;
                if (!gameObject.activeInHierarchy) continue;
         
                // If the ray intersects the object's box, add the hit to the list
                GameObjectRayHit gameObjectRayHit = null;
                if (gameObject.RaycastBox(ray, out gameObjectRayHit)) gameObjectHits.Add(gameObjectRayHit);
            }

            return gameObjectHits;
        }

        /// <summary>
        /// Performs a raycast and returns a list of hits for all sprite objects intersected
        /// by the ray.
        /// </summary>
        public List<GameObjectRayHit> RaycastAllSprite(Ray ray)
        {
            // First, retrieve a list of the sphere tree nodes which were hit by the ray.
            // If no nodes were hit, it means no object was hit either.
            List<SphereTreeNodeRayHit<GameObject>> allNodeHits = _sphereTree.RaycastAll(ray);
            if (allNodeHits.Count == 0) return new List<GameObjectRayHit>();

            // Loop through all nodes which were hit by the ray. For each node, we have to detect
            // if the ray hits the sprite object.
            var gameObjectHits = new List<GameObjectRayHit>();
            foreach (SphereTreeNodeRayHit<GameObject> nodeHit in allNodeHits)
            {
                // Retrieve the object which resides in the node
                GameObject gameObject = nodeHit.HitNode.Data;
                if (gameObject == null || !gameObject.activeInHierarchy) continue;
                if (!gameObject.HasSpriteRendererWithSprite()) continue;

                // If the ray intersects the object's sprite, add the hit to the list
                GameObjectRayHit gameObjectRayHit = null;
                if (gameObject.RaycastSprite(ray, out gameObjectRayHit)) gameObjectHits.Add(gameObjectRayHit);
            }

            return gameObjectHits;
        }

        /// <summary>
        /// Performs a raycast and returns a list of hits for all objects whose meshes 
        /// are intersected by the specified ray.
        /// </summary>
        public List<GameObjectRayHit> RaycastAllMesh(Ray ray)
        {
            // First, we will gather the objects whos boxes are intersected by the ray. If
            // no such objects exist, we will return an empty list.
            List<GameObjectRayHit> allBoxHits = RaycastAllBox(ray);
            if (allBoxHits.Count == 0) return new List<GameObjectRayHit>();

            // Now we will loop through all these objects and identify the ones whose meshes
            // are hit by the ray.
            var allMeshObjectHits = new List<GameObjectRayHit>(allBoxHits.Count);
            Octave3DMeshDatabase octave3DMeshDatabase = Octave3DMeshDatabase.Get();
            foreach(var boxHit in allBoxHits)
            {
                // Store the object for easy access
                GameObject hitObject = boxHit.HitObject;
                if (hitObject == null) continue;
                if (!hitObject.activeInHierarchy) continue;

                Renderer renderer = hitObject.GetComponent<Renderer>();
                if (renderer == null || !renderer.enabled) continue;

                // Store the object's mesh. If the object doesn't have a mesh, we will ignore it
                Mesh objectMesh = hitObject.GetMeshFromFilterOrSkinnedMeshRenderer();
                if (objectMesh == null) continue;

                // Check if the ray intersects the mesh
                Octave3DMesh octave3DMesh = octave3DMeshDatabase.GetOctave3DMesh(objectMesh);
                MeshRayHit meshRayHit = octave3DMesh.Raycast(ray, hitObject.transform.GetWorldMatrix());
             
                // If the mesh was hit by the ray, we will store the hit info inside the output array
                if (meshRayHit != null) allMeshObjectHits.Add(new GameObjectRayHit(ray, hitObject, null, meshRayHit, null, null));
            }

            return allMeshObjectHits;
        }

        /// <summary>
        /// Must be called from OnSceneGUI in order to perform any necessary updates.
        /// </summary>
        public void OnSceneGUI()
        {
            // If there are no objects registered, it means the scripts may have been recompiled
            // or this is a new Editor session. So we need to register the scene objects.
            if (_gameObjectToNode.Count == 0)
            {
                RegisterUnregisteredObjects();
            }

            _sphereTree.PerformPendingUpdates();
        }

        /// <summary>
        /// Must be called from an 'Update' method to perform any necessary tree
        /// adjustments.
        /// </summary>
        public void Update()
        {
            // According to the Unity docs, the 'Update' method is called when something changes 
            // in the scene, such as the transform fo an object so this seems like the best place 
            // to handle an object transform changes.
            HandleTransformChangesForAllRegisteredObjects();
        }

        /// <summary>
        /// Can be used to check if an object was registered with the tree.
        /// </summary>
        public bool ContainsGameObject(GameObject gameObject)
        {
            return _gameObjectToNode.ContainsKey(gameObject);
        }

        /// <summary>
        /// This method accepts a list of object hierarchy root nodes and registers
        /// all objects in those hierarchies with the tree.
        /// </summary>
        public void RegisterGameObjectHierarchies(List<GameObject> roots)
        {
            foreach(GameObject root in roots)
            {
                RegisterGameObjectHierarchy(root);
            }
        }

        /// <summary>
        /// Registers all objects in the specified hierarchy with the tree.
        /// </summary>
        public void RegisterGameObjectHierarchy(GameObject root)
        {
            List<GameObject> allChildrenIncludingSelf = root.GetAllChildrenIncludingSelf();
            foreach(GameObject gameObject in allChildrenIncludingSelf)
            {
                RegisterGameObject(gameObject);
            }
        }

        public void RegisterGameObjectCollection(List<GameObject> gameObjects)
        {
            foreach(var gameObject in gameObjects)
            {
                RegisterGameObject(gameObject);
            }
        }

        /// <summary>
        /// Registers the specified object with the tree.
        /// </summary>
        public void RegisterGameObject(GameObject gameObject)
        {
            Light[] lights = gameObject.GetComponents<Light>();
            foreach (var light in lights) Octave3DScene.Get().RegisterSceneLight(light);

            if (!CanGameObjectBeRegisteredWithTree(gameObject)) return;
            
            // Build the object's sphere
            Box objectWorldBox = gameObject.GetWorldBox();
            Sphere objectSphere = objectWorldBox.GetEncpasulatingSphere();
      
            // Add the object as a terminal node. Also store the node in the dictionary so that we can 
            // use it when it's needed.
            SphereTreeNode<GameObject> objectNode = _sphereTree.AddTerminalNode(objectSphere, gameObject);
            _gameObjectToNode.Add(gameObject, objectNode);
        }

        /// <summary>
        /// Handles any transform changes for the objects which were registered with the tree.
        /// </summary>
        public void HandleTransformChangesForAllRegisteredObjects()
        {
            // Note: When the tool object is not selected, we don't want to handle any transform changes because
            //       even though those changes will be registered with the tree, the 'OnSceneGUI' method will not
            //       be called and the result will be buggy and incorrect. So we will allow the user to move the
            //       objects around in the scene when the tool object is not selected, but we won't handle any
            //       transform changes. We will do that when the tool objects is selected again.
            // Note: We have to check for null in order to avoid a null reference exception which is thrown after
            //       the scripts have recompiled.
            if (Octave3DWorldBuilder.ActiveInstance != null && Selection.Contains(Octave3DWorldBuilder.ActiveInstance.gameObject))
            {
                // Loop through all object-to-nodes pairs
                foreach (var pair in _gameObjectToNode)
                {
                    // Can be null if the object was destroyed in the meantime
                    if (pair.Key == null) continue;

                    // If the object's transform has changed, handle the transform change
                    Transform gameObjectTransform = pair.Key.transform;
                    if (gameObjectTransform.hasChanged)
                    {
                        HandleObjectTransformChange(gameObjectTransform);

                        // Note: It's debatable whether or not this should be set to false here. If other
                        //       parts of the code will ever need this info, this needs to be eliminated.
                        gameObjectTransform.hasChanged = false;
                    }
                }
            }
        }

        public void Rebuild(bool showProgress)
        {
            if (Octave3DWorldBuilder.ActiveInstance == null) return;

            _sphereTree = new SphereTree<GameObject>(_numberOfChildNodesPerNode);
            _gameObjectToNode.Clear();
            _serializedNodes.Clear();

            List<GameObject> allWorkingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
            if(showProgress)
            {
                float invObjCount = 1.0f / (float)allWorkingObjects.Count;
                int numObjects = allWorkingObjects.Count;
                for (int objIndex = 0; objIndex < numObjects; ++objIndex)
                {
                    EditorUtility.DisplayProgressBar("Building tree", "Building game object tree. Please wait...", ((float)(objIndex + 1) * invObjCount));
                    RegisterGameObject(allWorkingObjects[objIndex]);
                }
                EditorUtility.ClearProgressBar();
            }
            else
            {
                int numObjects = allWorkingObjects.Count;
                for (int objIndex = 0; objIndex < numObjects; ++objIndex)
                {
                    RegisterGameObject(allWorkingObjects[objIndex]);
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns true if the specified game object can be registered with the tree.
        /// </summary>
        private bool CanGameObjectBeRegisteredWithTree(GameObject gameObject)
        {
            if (gameObject == null || _gameObjectToNode.ContainsKey(gameObject)) return false;
            //if (ObjectQueries.IsGameObjectEmpty(gameObject)) return false;
            if (!gameObject.HasMesh() && !gameObject.HasSpriteRenderer()) return false;
            if (!Octave3DWorldBuilder.ActiveInstance.IsWorkingObject(gameObject)) return false;
            if (Octave3DWorldBuilder.ActiveInstance.IsPivotWorkingObject(gameObject)) return false;
            if (gameObject.GetComponent<ObjectPlacementGuide>() != null) return false;
          
            return !gameObject.HasTerrain();
        }

        /// <summary>
        /// Handles the transform change for the specified object transform.
        /// </summary>
        private void HandleObjectTransformChange(Transform gameObjectTransform)
        {
            // Just ensure that the object is registered with the tree
            GameObject gameObject = gameObjectTransform.gameObject;
            if (!ContainsGameObject(gameObject)) return;

            // Store the object's node for easy access. We will need to instruct the 
            // tree to update this node as needed.
            SphereTreeNode<GameObject> objectNode = _gameObjectToNode[gameObject];

            // We will first have to detect what has changed. So we will compare the
            // object's sphere as it is now with what was before.
            bool updateCenter = false;
            bool updateRadius = false;
            Sphere previousSphere = objectNode.Sphere;
            Sphere currentSphere = gameObject.GetWorldBox().GetEncpasulatingSphere();

            // Detect what changed
            if (previousSphere.Center != currentSphere.Center) updateCenter = true;
            if (previousSphere.Radius != currentSphere.Radius) updateRadius = true;

            // Call the appropriate node update method
            if (updateCenter && updateRadius) _sphereTree.UpdateTerminalNodeCenterAndRadius(objectNode, currentSphere.Center, currentSphere.Radius);
            else if (updateCenter) _sphereTree.UpdateTerminalNodeCenter(objectNode, currentSphere.Center);
            else if (updateRadius) _sphereTree.UpdateTerminalNodeRadius(objectNode, currentSphere.Radius);
        }

        /// <summary>
        /// Callback which is called when the hierarchy window is changed in the Editor.
        /// </summary>
        private void HierarchyWindowChanged()
        {
            // The hierarchy window can change when objects are destroyed, so we will
            // call 'RemoveNullObjectNodes' to remove any null object nodes from the
            // sphere tree.
            RemoveNullObjectNodes();
            
            //RegisterUnregisteredObjects();
        }

        /// <summary>
        /// This method loops through all game objects which exist in the scene and registers them 
        /// with the tree if necessary. This is useful when the user decides to create objects using
        /// the Unity Editor interface because in that case the tool has no way of knowing that a new
        /// object was created.
        /// </summary>
        public void RegisterUnregisteredObjects()
        {
            // Note: It may be possible that the tool object was destroyed, so we have to check for null
            if(Octave3DWorldBuilder.ActiveInstance != null)
            {
                List<GameObject> workingObjects = Octave3DWorldBuilder.ActiveInstance.GetAllWorkingObjects();
                foreach (var childObject in workingObjects)
                {
                    if (!ContainsGameObject(childObject)) RegisterGameObject(childObject);
                }
            }
        }
        
        /// <summary>
        /// Removes any terminal nodes from the tree that have null object references.
        /// </summary>
        private void RemoveNullObjectNodes()
        {
            // Loop through each dictionaty entry
            var newObjectToNodeDictionary = new Dictionary<GameObject, SphereTreeNode<GameObject>>();
            foreach (var pair in _gameObjectToNode)
            {
                // If the key is null, remove the node, otherwise store this node in the new dictionary
                if (pair.Key == null) _sphereTree.RemoveNode(pair.Value);
                else newObjectToNodeDictionary.Add(pair.Key, pair.Value);
            }

            // Adjust the dictionary reference to point to the new one which doesn't contain any null object nodes.
            _gameObjectToNode = newObjectToNodeDictionary;
        }
        #endregion
    }
}
#endif