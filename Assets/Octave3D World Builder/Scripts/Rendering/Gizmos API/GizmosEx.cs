#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public static class GizmosEx
    {
        #region Private Static Variables
        private static Mesh _xyRectangleMesh;
        private static Mesh _xzRectangleMesh;

        private static Mesh _xyCircleMesh;
        private static int _numberOfCircleMeshSlices = 80;

        private static float _2DPointOffsetFromCamNearPlane = 0.01f;
        #endregion

        public static Mesh XZRectangleMesh
        {
            get
            {
                if (_xzRectangleMesh == null) _xzRectangleMesh = Octave3DWorldBuilder.ActiveInstance.ToolResources.MeshResources.CreateXZRectangleMesh(1.0f, 1.0f, Color.white);
                return _xzRectangleMesh;
            }
        }

        #region Public Static Functions
        public static void Render2DFilledSquare(Square2D square, Color color)
        {
            Render2DFilledRectangle(square.ToRectangle(), color);
        }

        public static void Render2DFilledRectangle(Rect rectangle, Color color)
        {
            Camera camera = SceneViewCamera.Camera;
            Transform cameraTransform = camera.transform;

            List<Vector3> rectWorldPoints = rectangle.GetWorldCornerPointsInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane);
            float width = (rectWorldPoints[0] - rectWorldPoints[1]).magnitude;
            float height = (rectWorldPoints[1] - rectWorldPoints[2]).magnitude;

            Vector3 rectangleWorldPosition = rectangle.GetWorldCenterPointInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane);

            Matrix4x4 transformMatrix = Matrix4x4.TRS(rectangleWorldPosition, cameraTransform.rotation, new Vector3(width, height, 1.0f));
            RenderMesh(GetRectangleMesh(), transformMatrix, color);
        }

        public static void Render2DSquareBorderLines(Square2D square, Color color)
        {
            Render2DRectangleBorderLines(square.ToRectangle(), color);
        }

        public static void Render2DRectangleBorderLines(Rect rectangle, Color color)
        {
            Camera camera = SceneViewCamera.Camera;
            List<Vector3> rectWorldPoints = rectangle.GetWorldCornerPointsInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane);

            RenderLinesBetweenPoints(rectWorldPoints, color);
        }

        public static void Render2DFilledCircle(Circle2D circle, Color color)
        {
            Render2DFilledEllipse(new Ellipse2D(circle), color);
        }

        public static void Render2DFilledEllipse(Ellipse2D ellipse, Color color)
        {
            Camera camera = SceneViewCamera.Camera;
            Transform cameraTransform = camera.transform;

            List<Vector3> ellipseWorldPoints = ellipse.GetWorldExtentPointsInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane);
            float radiusX = (ellipseWorldPoints[1] - ellipseWorldPoints[3]).magnitude * 0.5f;
            float radiusY = (ellipseWorldPoints[0] - ellipseWorldPoints[2]).magnitude * 0.5f;

            Vector3 ellipseWorldPosition = ellipse.GetWorldCenterPointInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane);
            Matrix4x4 transformMatrix = Matrix4x4.TRS(ellipseWorldPosition, cameraTransform.rotation, new Vector3(radiusX, radiusY, 1.0f));
            RenderMesh(GetCircleMesh(), transformMatrix, color);
        }

        public static void Render2DCircleBorderLines(Circle2D circle, Color color)
        {
            Ellipse2D ellipse = new Ellipse2D(circle);
            Render2DEllipseBorderLines(ellipse, color);
        }

        public static void Render2DEllipseBorderLines(Ellipse2D ellipse, Color color)
        {
            Camera camera = SceneViewCamera.Camera;
            List<Vector3> worldBorderPoints = ellipse.GetWorldBorderLinesInFrontOfCamera(camera, _2DPointOffsetFromCamNearPlane, _numberOfCircleMeshSlices);
 
            RenderLinesBetweenPoints(worldBorderPoints, color);
        }

        public static void RenderXZOrientedQuad(XZOrientedQuad3D orientedQuad, Color color)
        {
            GizmosMatrix.Push(orientedQuad.TransformMatrix.ToMatrix4x4x);
            GizmosColor.Push(color);

            Gizmos.DrawCube(Vector3.zero, new Vector3(orientedQuad.ModelSpaceXZSize.x, 0.0f, orientedQuad.ModelSpaceXZSize.y));

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderXZOrientedQuad(XZOrientedQuad3D orientedQuad, Color color, float offsetAlongNormal)
        {
            Matrix4x4 translationMatrix = Matrix4x4.TRS(orientedQuad.Plane.normal * offsetAlongNormal, Quaternion.identity, Vector3.one);
            GizmosMatrix.Push(translationMatrix * orientedQuad.TransformMatrix.ToMatrix4x4x);
            GizmosColor.Push(color);

            Gizmos.DrawCube(Vector3.zero, new Vector3(orientedQuad.ModelSpaceXZSize.x, 0.0f, orientedQuad.ModelSpaceXZSize.y));

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderXZOrientedQuadBorderLines(XZOrientedQuad3D orientedQuad, Color color)
        {
            RenderLinesBetweenPoints(orientedQuad.GetCornerPoints(), color);
        }

        public static void RenderXZOrientedQuadBorderLines(XZOrientedQuad3D orientedQuad, Color color, float offsetAlongNormal)
        {
            List<Vector3> cornerPoints = orientedQuad.GetCornerPoints();
            cornerPoints = Vector3Extensions.ApplyOffsetToPoints(cornerPoints, orientedQuad.Plane.normal * offsetAlongNormal);
            RenderLinesBetweenPoints(cornerPoints, color);
        }

        public static void RenderOrientedBox(OrientedBox orientedBox, Color color)
        {
            Matrix4x4 transformMatrix = Matrix4x4.TRS(orientedBox.Center, orientedBox.Rotation, orientedBox.ScaledSize);

            GizmosMatrix.Push(transformMatrix);
            GizmosColor.Push(color);

            Gizmos.DrawCube(Vector3.zero, Vector3.one);

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderBoxEdges(Box box, Color color)
        {
            GizmosColor.Push(color);
            Gizmos.DrawWireCube(box.Center, box.Size);
            GizmosColor.Pop();
        }

        public static void RenderOrientedBoxEdges(OrientedBox orientedBox, Color color)
        {
            GizmosColor.Push(color);
            GizmosMatrix.Push(Matrix4x4.TRS(orientedBox.Center, orientedBox.Rotation, orientedBox.ScaledSize));

            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);

            GizmosMatrix.Pop();
            GizmosColor.Pop();
        }

        public static void RenderOrientedBoxCornerEdges(OrientedBox orientedBox, float cornerEdgeLengthPercentage, Color color)
        {
            cornerEdgeLengthPercentage = Mathf.Clamp(cornerEdgeLengthPercentage, 0.0f, 1.0f);
            List<Vector3> boxCornerPoints = orientedBox.GetCenterAndCornerPoints();
            GizmosColor.Push(color);

            // Render the corner edges along the top edge of the box's front face
            Segment3D segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontTopLeft], boxCornerPoints[(int)BoxPoint.FrontTopRight]);
            float edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the bottom edge of the box's front face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontBottomLeft], boxCornerPoints[(int)BoxPoint.FrontBottomRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the left edge of the box's front face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontTopLeft], boxCornerPoints[(int)BoxPoint.FrontBottomLeft]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the right edge of the box's front face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontTopRight], boxCornerPoints[(int)BoxPoint.FrontBottomRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the top edge of the box's back face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.BackTopLeft], boxCornerPoints[(int)BoxPoint.BackTopRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the bottom edge of the box's back face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.BackBottomLeft], boxCornerPoints[(int)BoxPoint.BackBottomRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the left edge of the box's back face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.BackTopLeft], boxCornerPoints[(int)BoxPoint.BackBottomLeft]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the right edge of the box's back face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.BackTopRight], boxCornerPoints[(int)BoxPoint.BackBottomRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the left edge of the box's top face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontTopLeft], boxCornerPoints[(int)BoxPoint.BackTopRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the right edge of the box's top face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontTopRight], boxCornerPoints[(int)BoxPoint.BackTopLeft]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the left edge of the box's bottom face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontBottomLeft], boxCornerPoints[(int)BoxPoint.BackBottomRight]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            // Render the corner edges along the right edge of the box's bottom face
            segment = new Segment3D(boxCornerPoints[(int)BoxPoint.FrontBottomRight], boxCornerPoints[(int)BoxPoint.BackBottomLeft]);
            edgeLength = segment.HalfLength * cornerEdgeLengthPercentage;
            if (edgeLength > 0.0f)
            {
                Gizmos.DrawLine(segment.StartPoint, segment.StartPoint + segment.NormalizedDirection * edgeLength);
                Gizmos.DrawLine(segment.EndPoint, segment.EndPoint - segment.NormalizedDirection * edgeLength);
            }

            GizmosColor.Pop();
        }

        public static void RenderMesh(Mesh mesh, Matrix4x4 transformMatrix, Color color)
        {
            GizmosMatrix.Push(transformMatrix);
            GizmosColor.Push(color);

            Gizmos.DrawMesh(mesh);

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderLinesBetweenPoints(List<Vector3> points, Color color)
        {
            GizmosMatrix.Push(Matrix4x4.identity);
            GizmosColor.Push(color);

            int numberOfPoints = points.Count;
            for (int pointIndex = 0; pointIndex < numberOfPoints; ++pointIndex)
            {
                Gizmos.DrawLine(points[pointIndex], points[(pointIndex + 1) % numberOfPoints]);
            }

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderLinesBetweenPoints(List<Vector3> points, Color color, Vector3 offset)
        {
            GizmosMatrix.Push(Matrix4x4.identity);
            GizmosColor.Push(color);

            int numberOfPoints = points.Count;
            for (int pointIndex = 0; pointIndex < numberOfPoints; ++pointIndex)
            {
                Gizmos.DrawLine(points[pointIndex] + offset, points[(pointIndex + 1) % numberOfPoints] + offset);
            }

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }

        public static void RenderLine(Vector3 firstPoint, Vector3 secondPoint, Color color)
        {
            GizmosMatrix.Push(Matrix4x4.identity);
            GizmosColor.Push(color);

            Gizmos.DrawLine(firstPoint, secondPoint);

            GizmosColor.Pop();
            GizmosMatrix.Pop();
        }
        #endregion

        #region Private Static Functions
        private static Mesh GetRectangleMesh()
        {
            if (_xyRectangleMesh == null) _xyRectangleMesh = Octave3DWorldBuilder.ActiveInstance.ToolResources.MeshResources.CreateXYRectangleMesh(1.0f, 1.0f, Color.white);
            return _xyRectangleMesh;
        }

        private static Mesh GetCircleMesh()
        {
            if (_xyCircleMesh == null) _xyCircleMesh = Octave3DWorldBuilder.ActiveInstance.ToolResources.MeshResources.CreateXYCircleMesh(1.0f, _numberOfCircleMeshSlices, Color.white);
            return _xyCircleMesh;
        }
        #endregion
    }
}
#endif