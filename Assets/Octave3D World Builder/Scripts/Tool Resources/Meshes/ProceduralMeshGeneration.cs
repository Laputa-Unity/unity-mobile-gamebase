#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ProceduralMeshGeneration
    {
        #region Public Static Properties
        public static float MinAllowedDimensionValue { get { return 1e-5f; } }
        #endregion

        #region Public Static Functions
        public static Mesh CreateXYRectangleMesh(float rectangleWidth, float rectangleHeight, Color rectangleColor)
        {
            if (!ValidateXYRectangleDimensions(rectangleWidth, rectangleHeight)) return null;

            var vertexPositions = Vector3Extensions.Get3DRectangleCircumferencePoints(rectangleWidth, rectangleHeight, Vector3.right, Vector3.up, Vector3.zero);
            var vertexColors = ListExtensions.GetFilledList(rectangleColor, 4);
            var vertexNormals = ListExtensions.GetFilledList(-Vector3.forward, 4);
            var vertexIndices = GenerateXYRectangleVertexIndices();

            return ProceduralMeshFactory.CreateTriangleMesh(vertexPositions.ToArray(), vertexIndices, vertexColors.ToArray(), vertexNormals.ToArray());
        }

        public static Mesh CreateXZRectangleMesh(float rectangleWidth, float rectangleDepth, Color rectangleColor)
        {
            if (!ValidateXYRectangleDimensions(rectangleWidth, rectangleDepth)) return null;

            var vertexPositions = Vector3Extensions.Get3DRectangleCircumferencePoints(rectangleWidth, rectangleDepth, Vector3.right, Vector3.forward, Vector3.zero);
            var vertexColors = ListExtensions.GetFilledList(rectangleColor, 4);
            var vertexNormals = ListExtensions.GetFilledList(Vector3.up, 4);
            var vertexIndices = GenerateXZRectangleVertexIndices();

            return ProceduralMeshFactory.CreateTriangleMesh(vertexPositions.ToArray(), vertexIndices, vertexColors.ToArray(), vertexNormals.ToArray());
        }

        public static Mesh CreateXYCircleMesh(float circleRadius, int numberOfCircleSlices, Color circleColor)
        {
            if (!ValidateXYCircleRadiusAndSlices(circleRadius, numberOfCircleSlices)) return null;

            var vertexPositions = GenerateXYCircleVertexPositions(circleRadius, numberOfCircleSlices);
            var vertexColors = ListExtensions.GetFilledList(circleColor, vertexPositions.Count);
            var vertexNormals = ListExtensions.GetFilledList(-Vector3.forward, vertexPositions.Count);
            var vertexIndices = GenerateXYCircleVertexIndices(numberOfCircleSlices);

            return ProceduralMeshFactory.CreateTriangleMesh(vertexPositions.ToArray(), vertexIndices, vertexColors.ToArray(), vertexNormals.ToArray());
        }
        #endregion

        #region Private Static Functions
        private static bool ValidateXYRectangleDimensions(float rectangleWidth, float rectangleHeight)
        {
            return rectangleWidth >= MinAllowedDimensionValue && rectangleHeight >+ MinAllowedDimensionValue;
        }

        private static int[] GenerateXYRectangleVertexIndices()
        {
            return new int[] { 0, 1, 2, 0, 2, 3 };
        }

        private static int[] GenerateXZRectangleVertexIndices()
        {
            return new int[] { 0, 1, 2, 0, 2, 3 };
        }

        private static bool ValidateXYCircleRadiusAndSlices(float circleRadius, int numberOfCircleSlices)
        {
            return circleRadius >= MinAllowedDimensionValue && numberOfCircleSlices >= 4;
        }

        private static List<Vector3> GenerateXYCircleVertexPositions(float circleRadius, int numberOfCircleSlices)
        {
            int numberOfCircumferenceVerts = numberOfCircleSlices + 1;
            int totalNumberOfVerts = numberOfCircumferenceVerts + 1;

            var vertexPositions = new List<Vector3>(totalNumberOfVerts);
            vertexPositions.Add(Vector3.zero);
            vertexPositions.AddRange(Vector3Extensions.Get3DEllipseCircumferencePoints(circleRadius, circleRadius, Vector3.right, Vector3.up, Vector3.zero, numberOfCircleSlices));

            return vertexPositions;
        }

        private static int[] GenerateXYCircleVertexIndices(int numberOfCircleSlices)
        {
            int numberOfTriangles = numberOfCircleSlices;
            int numberOfVertexIndices = numberOfTriangles * 3;
            var vertexIndices = new int[numberOfVertexIndices];

            for (int triangleIndex = 0; triangleIndex < numberOfTriangles; ++triangleIndex)
            {
                vertexIndices[triangleIndex * 3] = 0;
                vertexIndices[triangleIndex * 3 + 1] = triangleIndex + 1;
                vertexIndices[triangleIndex * 3 + 2] = triangleIndex + 2;
            }

            return vertexIndices;
        }
        #endregion
    }
}
#endif