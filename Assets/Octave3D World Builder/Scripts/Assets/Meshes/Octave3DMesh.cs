#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public class MeshTriangleInfo
    {
        public int[] VertIndices = new int[3];
        public Triangle3D ModelSpaceTriangle;
    }

    [Serializable]
    public class Octave3DMesh
    {
        #region Private Variables
        [SerializeField]
        private Mesh _mesh;
        [SerializeField]
        private Vector3[] _vertexPositions;
        [SerializeField]
        private int[] _vertexIndices;
        [SerializeField]
        private int _numberOfTriangles;

        [NonSerialized]
        private MeshSphereTree _meshSphereTree;
        #endregion

        #region Public Properties
        public Mesh Mesh { get { return _mesh; } }
        public int NumberOfTriangles { get { return _numberOfTriangles; } }
        public Vector3[] VertexPositions { get { return _vertexPositions.Clone() as Vector3[]; } }
        public int[] VertexIndices { get { return _vertexIndices.Clone() as int[]; } }
        public Box ModelAABB { get { return _mesh != null ? new Box(_mesh.bounds) : Box.GetInvalid(); } }
        #endregion

        #region Constructors
        public Octave3DMesh()
        {
            _meshSphereTree = new MeshSphereTree(this);
        }

        public Octave3DMesh(Mesh mesh)
        {
            _mesh = mesh;
            _vertexPositions = _mesh.vertices;
            _vertexIndices = _mesh.triangles;
            _numberOfTriangles = (int)(_vertexIndices.Length / 3);

            _meshSphereTree = new MeshSphereTree(this);
        }
        #endregion

        #region Public Methods
        public Box GetBox()
        {
            if (_mesh == null) return Box.GetInvalid();
            return new Box(_mesh.bounds);
        }

        public OrientedBox GetOrientedBox(TransformMatrix transformMatrix)
        {
            if (_mesh == null) return OrientedBox.GetInvalid();

            OrientedBox orientedBox = new OrientedBox(GetBox());
            orientedBox.Transform(transformMatrix);

            return orientedBox;
        }

        public void RenderGizmosDebug(TransformMatrix meshTransformMatrix)
        {
            _meshSphereTree.RenderGizmosDebug(meshTransformMatrix);
        }

        public MeshTriangleInfo GetMeshTriangleInfo(int triangleIndex)
        {
            int baseIndex = triangleIndex * 3;

            var triangleInfo = new MeshTriangleInfo();
            triangleInfo.VertIndices[0] = _vertexIndices[baseIndex];
            triangleInfo.VertIndices[1] = _vertexIndices[baseIndex + 1];
            triangleInfo.VertIndices[2] = _vertexIndices[baseIndex + 2];
            triangleInfo.ModelSpaceTriangle = new Triangle3D(_vertexPositions[triangleInfo.VertIndices[0]], _vertexPositions[triangleInfo.VertIndices[1]], _vertexPositions[triangleInfo.VertIndices[2]]);

            return triangleInfo;
        }

        public Triangle3D GetTriangle(int triangleIndex)
        {
            int baseIndex = triangleIndex * 3;
            return new Triangle3D(_vertexPositions[_vertexIndices[baseIndex]], _vertexPositions[_vertexIndices[baseIndex + 1]], _vertexPositions[_vertexIndices[baseIndex + 2]]);
        }

        public MeshRayHit Raycast(Ray ray, TransformMatrix meshTransformMatrix)
        {
            //meshTransformMatrix.Scale = meshTransformMatrix.Scale.GetVectorWithPositiveComponents();
            return _meshSphereTree.Raycast(ray, meshTransformMatrix);
        }

        public List<Vector3> GetOverlappedModelVerts(Box modelOverlapBox)
        {
            return _meshSphereTree.GetOverlappedModelVerts(modelOverlapBox);
        }

        public List<Vector3> GetOverlappedWorldVerts(Box worldOverlapBox, TransformMatrix meshTransformMatrix)
        {
            return _meshSphereTree.GetOverlappedWorldVerts(worldOverlapBox.ToOrientedBox(), meshTransformMatrix);
        }

        public List<Vector3> GetOverlappedWorldVerts(OrientedBox box, TransformMatrix meshTransformMatrix)
        {
            return _meshSphereTree.GetOverlappedWorldVerts(box, meshTransformMatrix);
        }

        public List<Triangle3D> GetOverlappedTriangles(OrientedBox box, TransformMatrix meshTransformMatrix)
        {
            return _meshSphereTree.GetOverlappedWorldTriangles(box, meshTransformMatrix);
        }

        public bool AllTrianglesFaceAway(Camera camera, Transform meshTransform, bool forceOrtho)
        {
            int numTriangles = NumberOfTriangles;
            Transform camTransform = camera.transform;
            Vector3 camPos = camTransform.position;
            Vector3 camLook = camTransform.forward;

            if (camera.orthographic || forceOrtho)
            {
                for (int triIndex = 0; triIndex < numTriangles; ++triIndex)
                {
                    var tri = GetTriangle(triIndex);
                    var triNormal = meshTransform.TransformDirection(tri.Normal);

                    float dot = Vector3.Dot(triNormal, camLook);
                    if (dot < 0.0f) return false;
                }
            }
            else
            {
                for (int triIndex = 0; triIndex < numTriangles; ++triIndex)
                {
                    var tri = GetTriangle(triIndex);
                    float dot = Vector3.Dot(tri.Point0 - camPos, camLook);
                    if (dot < 0.0f) return false;
                }
            }

            return true;
        }

        public bool IntersectsMesh(TransformMatrix thisTransform, Octave3DMesh otherMesh, TransformMatrix otherTransform)
        {
            OrientedBox thisOOBB = new OrientedBox(ModelAABB);
            thisOOBB.Transform(thisTransform);
            OrientedBox otherOOBB = new OrientedBox(otherMesh.ModelAABB);
            otherOOBB.Transform(otherTransform);

            if (!thisOOBB.Intersects(otherOOBB)) return false;

            List<Triangle3D> thisTriangles = GetOverlappedTriangles(otherOOBB, thisTransform);
            for (int thisTriIndex = 0; thisTriIndex < thisTriangles.Count; ++thisTriIndex)
            {
                Triangle3D thisTri = thisTriangles[thisTriIndex];
                OrientedBox thisTriOOBB = new OrientedBox(thisTri.GetEncapsulatingBox());
                List<Triangle3D> otherTriangles = otherMesh.GetOverlappedTriangles(thisTriOOBB, otherTransform);
                for (int otherTriIndex = 0; otherTriIndex < otherTriangles.Count; ++otherTriIndex)
                {
                    Triangle3D otherTri = otherTriangles[otherTriIndex];
                    if (thisTri.IntersectsTriangle(otherTri)) return true;
                }
            }

            return false;
        }

        public OrientedBox GetOOBB(TransformMatrix transformMatrix)
        {
            if (_mesh == null) return OrientedBox.GetInvalid();

            OrientedBox oobb = new OrientedBox(ModelAABB);
            oobb.Transform(transformMatrix);

            return oobb;
        }

        public List<Triangle3DIntersectInfo> GetIntersectingTriangles(TransformMatrix thisTransform, Octave3DMesh otherMesh, TransformMatrix otherTransform)
        {
            OrientedBox thisOOBB = new OrientedBox(ModelAABB);
            thisOOBB.Transform(thisTransform);
            OrientedBox otherOOBB = new OrientedBox(otherMesh.ModelAABB);
            otherOOBB.Transform(otherTransform);

            if (thisOOBB.Intersects(otherOOBB))
            {
                List<Triangle3D> thisTriangles = GetOverlappedTriangles(otherOOBB, thisTransform);

                var output = new List<Triangle3DIntersectInfo>(50);
                Triangle3DIntersectInfo intersectInfo;
                for (int thisTriIndex = 0; thisTriIndex < thisTriangles.Count; ++thisTriIndex)
                {
                    Triangle3D thisTri = thisTriangles[thisTriIndex];

                    OrientedBox thisTriOOBB = new OrientedBox(thisTri.GetEncapsulatingBox());
                    List<Triangle3D> otherTriangles = otherMesh.GetOverlappedTriangles(thisTriOOBB, otherTransform);

                    for (int otherTriIndex = 0; otherTriIndex < otherTriangles.Count; ++otherTriIndex)
                    {
                        Triangle3D otherTri = otherTriangles[otherTriIndex];
                        if (thisTri.IntersectsTriangle(otherTri, out intersectInfo))
                        {
                            output.Add(intersectInfo);
                        }
                    }
                }

                return output;
            }
            else return new List<Triangle3DIntersectInfo>();
        }
        #endregion
    }
}
#endif