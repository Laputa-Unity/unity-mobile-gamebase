#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class Object2ObjectBoxSnapDataFactory
    {
        public static Object2ObjectBoxSnapData Create(GameObject meshObject)
        {
            if (meshObject == null) return null;

            Mesh objectMesh = meshObject.GetMeshFromFilterOrSkinnedMeshRenderer();
            if (objectMesh == null) return null;

            Renderer renderer = meshObject.GetRenderer();
            if (renderer == null || !renderer.enabled) return null;

            Octave3DMesh octaveMesh = Octave3DMeshDatabase.Get().GetOctave3DMesh(objectMesh);
            if (octaveMesh == null) return null;

            List<Box> modelVertOverlapBoxes = BuildModelVertOverlapBoxes(octaveMesh);
            var snapBoxIDs = Object2ObjectBoxSnapData.GetAllSnapBoxIDs();
            var modelSnapBoxes = new List<Box>(snapBoxIDs.Length);
            Box modelMeshBox = octaveMesh.ModelAABB;
            BoxFace[] meshBoxFaces = Object2ObjectBoxSnapData.GetBoxFaceToSnapBoxIDMap();
            foreach (var snapBox in snapBoxIDs)
            {
                Box overlapBox = modelVertOverlapBoxes[(int)snapBox];
                List<Vector3> overlappedVerts = octaveMesh.GetOverlappedModelVerts(overlapBox);
                Plane meshFacePlane = modelMeshBox.GetBoxFacePlane(meshBoxFaces[(int)snapBox]);
                overlappedVerts = meshFacePlane.ProjectAllPoints(overlappedVerts);

                modelSnapBoxes.Add(Box.FromPoints(overlappedVerts));
            }

            return new Object2ObjectBoxSnapData(meshObject, modelSnapBoxes);
        }

        private static List<Box> BuildModelVertOverlapBoxes(Octave3DMesh octaveMesh)
        {
            const float overlapAmount = 0.2f;
            float halfOverlapAmount = overlapAmount * 0.5f;
            Box modelMeshBox = octaveMesh.ModelAABB;
            Vector3 meshBoxSize = modelMeshBox.Size;
            BoxFace[] meshBoxFaces = Object2ObjectBoxSnapData.GetBoxFaceToSnapBoxIDMap();

            // Must have 1 to 1 mapping with Object2ObjectBoxSnapData.SnapBox enum
            const float sizeEps = 0.001f;
            Vector3[] overlapBoxSizes = new Vector3[]
            {
                // Left and right
                new Vector3(overlapAmount, meshBoxSize.y + sizeEps, meshBoxSize.z + sizeEps),
                new Vector3(overlapAmount, meshBoxSize.y + sizeEps, meshBoxSize.z + sizeEps),

                // Bottom and top
                new Vector3(meshBoxSize.x + sizeEps, overlapAmount, meshBoxSize.z + sizeEps),
                new Vector3(meshBoxSize.x + sizeEps, overlapAmount, meshBoxSize.z + sizeEps),

                // Back and front
                new Vector3(meshBoxSize.x + sizeEps, meshBoxSize.y + sizeEps, overlapAmount),
                new Vector3(meshBoxSize.x + sizeEps, meshBoxSize.y + sizeEps, overlapAmount),
            };

            var overlapBoxes = new List<Box>();
            for (int boxFaceIndex = 0; boxFaceIndex < meshBoxFaces.Length; ++boxFaceIndex)
            {
                BoxFace meshBoxFace = meshBoxFaces[boxFaceIndex];
                Vector3 faceCenter = modelMeshBox.GetBoxFaceCenter(meshBoxFace);
                Vector3 faceNormal = modelMeshBox.GetBoxFacePlane(meshBoxFace).normal;
                Vector3 overlapCenter = faceCenter - faceNormal * halfOverlapAmount;
                overlapBoxes.Add(new Box(overlapCenter, overlapBoxSizes[boxFaceIndex]));
            }

            return overlapBoxes;
        }
    }
}
#endif