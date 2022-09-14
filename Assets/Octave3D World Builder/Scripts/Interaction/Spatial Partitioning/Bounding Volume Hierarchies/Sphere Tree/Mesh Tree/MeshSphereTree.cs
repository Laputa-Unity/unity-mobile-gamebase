#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    /// <summary>
    /// A mesh sphere tree which can be used to speed up different kinds of
    /// mesh queries such as raycasts, overlaps etc.
    /// </summary>
    public class MeshSphereTree 
    {
        #region Private Variables
        /// <summary>
        /// The sphere tree. The terminal nodes in this tree will contain the triangles
        /// inside the associated mesh.
        /// </summary>
        private SphereTree<MeshSphereTreeTriangle> _sphereTree;

        /// <summary>
        /// The mesh associated with the tree.
        /// </summary>
        private Octave3DMesh _octave3DMesh;

        /// <summary>
        /// We will postpone the construction of the tree until its data is actually needed. This
        /// boolean will help us do that.
        /// </summary>
        private bool _wasBuilt = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="octave3DMesh">
        /// The mesh associated with the tree.
        /// </param>
        public MeshSphereTree(Octave3DMesh octave3DMesh)
        {
            _octave3DMesh = octave3DMesh;
            _sphereTree = new SphereTree<MeshSphereTreeTriangle>(2);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Renders the tree gizmos in the scene view for debugging purposes.
        /// </summary>
        /// <param name="meshTransformMatrix">
        /// The transform matrix fo the mesh which is associated with the tree.
        /// </param>
        public void RenderGizmosDebug(TransformMatrix meshTransformMatrix)
        {
            // ToDO: Set scale to 1?
            _sphereTree.RenderGizmosDebug(meshTransformMatrix);
        }

        public List<Vector3> GetOverlappedModelVerts(Box modelBox)
        {
            if (!_wasBuilt) Build();

            OrientedBox meshSpaceOOBB = new OrientedBox(modelBox);
            List<SphereTreeNode<MeshSphereTreeTriangle>> overlappedNodes = _sphereTree.OverlapBox(meshSpaceOOBB);
            if (overlappedNodes.Count == 0) return new List<Vector3>();

            HashSet<int> usedIndices = new HashSet<int>();
            var overlappedModelVerts = new List<Vector3>(50);
            foreach (var node in overlappedNodes)
            {
                int triangleIndex = node.Data.TriangleIndex;

                MeshTriangleInfo triangleInfo = _octave3DMesh.GetMeshTriangleInfo(triangleIndex);
                List<Vector3> trianglePoints = triangleInfo.ModelSpaceTriangle.GetPoints();

                for (int ptIndex = 0; ptIndex < trianglePoints.Count; ++ptIndex)
                {
                    if (usedIndices.Contains(triangleInfo.VertIndices[ptIndex])) continue;

                    Vector3 point = trianglePoints[ptIndex];
                    if (meshSpaceOOBB.ContainsPoint(point))
                    {
                        overlappedModelVerts.Add(point);
                        usedIndices.Add(triangleInfo.VertIndices[ptIndex]);
                    }
                }
            }

            return overlappedModelVerts;
        }

        /// <summary>
        /// Returns the world space vertices overlapped by the specified box.
        /// </summary>
        public List<Vector3> GetOverlappedWorldVerts(OrientedBox box, TransformMatrix meshTransformMatrix)
        {
            // If the tree was not yet build, we need to build it because we need 
            // the triangle information in order to perform the raycast.
            if (!_wasBuilt) Build();

            // Work in mesh model space because the tree data exists in model space
            OrientedBox meshSpaceOOBB = new OrientedBox(box);
            Matrix4x4 inverseTransform = meshTransformMatrix.ToMatrix4x4x.inverse;
            meshSpaceOOBB.Transform(inverseTransform);

            // Used to avoid duplicate indices since there can be triangles which share the same vertices
            // and we don't want to return the same vertex multiple times.
            HashSet<int> usedIndices = new HashSet<int>();

            // Retrieve the nodes overlapped by the specified box
            List<SphereTreeNode<MeshSphereTreeTriangle>> overlappedNodes = _sphereTree.OverlapBox(meshSpaceOOBB);
            if (overlappedNodes.Count == 0) return new List<Vector3>();

            // Loop through all nodes
            var overlappedWorldVerts = new List<Vector3>(50);
            foreach(var node in overlappedNodes)
            {
                // Get the triangle associated with the node
                int triangleIndex = node.Data.TriangleIndex;
                
                MeshTriangleInfo triangleInfo = _octave3DMesh.GetMeshTriangleInfo(triangleIndex);
                List<Vector3> trianglePoints = triangleInfo.ModelSpaceTriangle.GetPoints();

                for(int ptIndex = 0; ptIndex < trianglePoints.Count; ++ptIndex)
                {
                    if (usedIndices.Contains(triangleInfo.VertIndices[ptIndex])) continue;

                    Vector3 point = trianglePoints[ptIndex];
                    if (meshSpaceOOBB.ContainsPoint(point))
                    {
                        overlappedWorldVerts.Add(meshTransformMatrix.MultiplyPoint(point));
                        usedIndices.Add(triangleInfo.VertIndices[ptIndex]);
                    }                  
                }

                /*Triangle3D modelSpaceTriangle = _octave3DMesh.GetTriangle(triangleIndex);

                // Now check which of the triangle points resides inside the box
                List<Vector3> trianglePoints = modelSpaceTriangle.GetPoints();
                foreach(var pt in trianglePoints)
                {
                    // When a point resides inside the box, we will transform it in world space and add it to the final point list
                    if (box.ContainsPoint(pt)) overlappedWorldVerts.Add(meshTransformMatrix.MultiplyPoint(pt));
                }*/
            }

            return overlappedWorldVerts;
        }

        public List<Triangle3D> GetOverlappedWorldTriangles(OrientedBox box, TransformMatrix meshTransformMatrix)
        {
            // If the tree was not yet build, we need to build it because we need 
            // the triangle information in order to perform the raycast.
            if (!_wasBuilt) Build();

            // Work in mesh model space because the tree data exists in model space
            OrientedBox meshSpaceBox = new OrientedBox(box);
            Matrix4x4 inverseTransform = meshTransformMatrix.ToMatrix4x4x.inverse;
            meshSpaceBox.Transform(inverseTransform);

            List<SphereTreeNode<MeshSphereTreeTriangle>> overlappedNodes = _sphereTree.OverlapBox(meshSpaceBox);
            if (overlappedNodes.Count == 0) return new List<Triangle3D>();

            Box queryBox = Box.FromPoints(meshSpaceBox.GetCenterAndCornerPoints());
            var overlappedWorldTriangles = new List<Triangle3D>(50);
            foreach (var node in overlappedNodes)
            {
                int triangleIndex = node.Data.TriangleIndex;
                MeshTriangleInfo triangleInfo = _octave3DMesh.GetMeshTriangleInfo(triangleIndex);

                if (triangleInfo.ModelSpaceTriangle.IntersectsBox(queryBox))
                {
                    triangleInfo.ModelSpaceTriangle.TransformPoints(meshTransformMatrix);
                    overlappedWorldTriangles.Add(triangleInfo.ModelSpaceTriangle);
                }
            }

            return overlappedWorldTriangles;
        }

        /// <summary>
        /// Performs a ray cast against the mesh tree and returns an instance of the 'MeshRayHit'
        /// class which holds information about the ray hit. The method returns the hit which is
        /// closest to the ray origin. If no triangle was hit, the method returns null.
        /// </summary>
        public MeshRayHit Raycast(Ray ray, TransformMatrix meshTransformMatrix)
        {
            // If the tree was not yet build, we need to build it because we need 
            // the triangle information in order to perform the raycast.
            if (!_wasBuilt) Build();
          
            // When the sphere tree is constructed it is constructed in the mesh local space (i.e. it takes
            // no position/rotation/scale into account). This is required because a mesh can be shared by
            // lots of different objects each with its own transform data. This is why we need the mes matrix
            // parameter. It allows us to transform the ray in the mesh local space and perform our tests there.
            Ray meshLocalSpaceRay = ray.InverseTransform(meshTransformMatrix.ToMatrix4x4x);

            // First collect all terminal nodes which are intersected by this ray. If no nodes
            // are intersected, we will return null.
            List<SphereTreeNodeRayHit<MeshSphereTreeTriangle>> nodeRayHits = _sphereTree.RaycastAll(meshLocalSpaceRay);
            if (nodeRayHits.Count == 0) return null;

            // We now have to loop thorugh all intersected nodes and find the triangle whose
            // intersection point is closest to the ray origin.
            float minT = float.MaxValue; 
            Triangle3D closestTriangle = null;
            int indexOfClosestTriangle = -1;
            Vector3 closestHitPoint = Vector3.zero;
            foreach(var nodeRayHit in nodeRayHits)
            {
                // Retrieve the data associated with the node and construct the mesh triangle instance
                MeshSphereTreeTriangle sphereTreeTriangle = nodeRayHit.HitNode.Data;
                Triangle3D meshTriangle = _octave3DMesh.GetTriangle(sphereTreeTriangle.TriangleIndex);
 
                // Check if the ray intersects the trianlge which resides in the node
                float hitEnter;
                if(meshTriangle.Raycast(meshLocalSpaceRay, out hitEnter))
                {
                    // The trianlge is intersected by the ray, but we also have to ensure that the
                    // intersection point is closer than what we have found so far. If it is, we 
                    // store all relevant information.
                    if(hitEnter < minT)
                    {
                        minT = hitEnter;
                        closestTriangle = meshTriangle;
                        indexOfClosestTriangle = sphereTreeTriangle.TriangleIndex;
                        closestHitPoint = meshLocalSpaceRay.GetPoint(hitEnter);
                    }
                }
            }

            // If we found the closest triangle, we can construct the ray hit instance and return it.
            // Otherwise we return null. This can happen when the ray intersects the triangle node
            // spheres, but the triangles themselves.
            if (closestTriangle != null)
            {
                // We have worked in mesh local space up until this point, but we want to return the
                // hit info in world space, so we have to transform the hit data accordingly.
                closestHitPoint = meshTransformMatrix.MultiplyPoint(closestHitPoint);
                minT = (ray.origin - closestHitPoint).magnitude;
                Vector3 worldNormal = meshTransformMatrix.MultiplyVector(closestTriangle.Normal);

                return new MeshRayHit(ray, minT, _octave3DMesh, indexOfClosestTriangle, closestHitPoint, worldNormal, meshTransformMatrix);
            }
            else return null;
        }

        /// <summary>
        /// Builds the mesh tree if it hasn't already been built.
        /// </summary>
        public void Build()
        {
            if (_wasBuilt) return;

            // Loop through all trianlges and register them with the tree
            for(int triangleIndex = 0; triangleIndex < _octave3DMesh.NumberOfTriangles; ++triangleIndex)
            {
                RegisterTriangle(triangleIndex);
            }

            // Finish any pending updates.
            // Note: This assumes that mesh data will never be modified during editing. Otherwise, we
            //       might need to call this from 'OnSceneGUI' to ensure that any pending updates are
            //       performed accordingly.
            _sphereTree.PerformPendingUpdates();

            // The tree was built
            _wasBuilt = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Registers the mesh trianlge with the specified index with the tree.
        /// </summary>
        /// <returns>
        /// True if the trianlge was registered and false otherwise. The only 
        /// scenario in which the method can return false is when the triangle
        /// is degenerate.
        /// </returns>
        private bool RegisterTriangle(int triangleIndex)
        {
            // Retrieve the triangle from the mesh. If it is degenerate, we return false.
            Triangle3D triangle = _octave3DMesh.GetTriangle(triangleIndex);
            if (triangle.IsDegenerate) return false;
           
            // Create the triangle node data and instruct the tree to add this node
            var meshSphereTreeTriangle = new MeshSphereTreeTriangle(triangleIndex);
            _sphereTree.AddTerminalNode(triangle.GetEncapsulatingSphere(), meshSphereTreeTriangle);

            return true;
        }
        #endregion
    }
}
#endif