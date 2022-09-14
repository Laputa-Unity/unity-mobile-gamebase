#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    [Serializable]
    public class XZOrientedQuad3D
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _xzSize = Vector2.zero;
        [SerializeField]
        private CoordinateSystem _coordinateSystem = new CoordinateSystem();
        #endregion

        #region Public Static Properties
        public static Vector3 ModelSpaceRightAxis { get { return Vector3.right; } }
        public static Vector3 ModelSpacePlaneNormal { get { return Vector3.up; } }
        public static Vector3 ModelSpaceLookAxis { get { return Vector3.forward; } }
        #endregion

        #region Public Properties
        public Vector2 ModelSpaceXZSize { get { return _xzSize; } set { _xzSize = value.GetVectorWithAbsoluteValueComponents(); } }
        public Vector2 ScaledXZSize { get { return new Vector2(_xzSize.x * _coordinateSystem.TransformMatrix.XScale, _xzSize.y * _coordinateSystem.TransformMatrix.ZScale); } }
        public float SizeOnBothXZAxes { set { _xzSize = new Vector2(value, value); } }
        public Vector2 ModelSpaceXZExtents { get { return ModelSpaceXZSize * 0.5f; } }
        public Vector2 ScaledXZExtents { get { return ScaledXZSize * 0.5f; } }
        public Quaternion Rotation { get { return _coordinateSystem.GetRotation(); } set { _coordinateSystem.SetRotation(value); } }
        public Vector3 Center
        {
            get { return _coordinateSystem.GetOriginPosition(); }
            set { _coordinateSystem.SetOriginPosition(value); }
        }
        public Plane Plane { get { return new Plane(_coordinateSystem.GetAxisVector(CoordinateSystemAxis.PositiveUp), Center); } }
        public TransformMatrix TransformMatrix { get { return _coordinateSystem.TransformMatrix; } }
        public float ScaledArea
        {
            get
            {
                Vector2 scaledXZSize = ScaledXZSize;
                return scaledXZSize.x * scaledXZSize.y;
            }
        }
        #endregion

        #region Constructors
        public XZOrientedQuad3D()
        {
        }

        public XZOrientedQuad3D(Vector3 center)
        {
            Center = center;
        }

        public XZOrientedQuad3D(Vector3 center, Vector2 xzSize)
        {
            Center = center;
            ModelSpaceXZSize = xzSize;
        }

        public XZOrientedQuad3D(Vector3 center, Vector2 xzSize, Quaternion rotation)
        {
            Center = center;
            ModelSpaceXZSize = xzSize;
            Rotation = rotation;
        }

        public XZOrientedQuad3D(XZOrientedQuad3D source)
        {
            Center = source.Center;
            ModelSpaceXZSize = source.ModelSpaceXZSize;
            Rotation = source.Rotation;
        }
        #endregion

        #region Public Methods
        public void Transform(TransformMatrix transformMatrix)
        {
            _coordinateSystem.Transform(transformMatrix);
        }

        public void SetScale(float scale)
        {
            _coordinateSystem.SetScaleOnAllAxes(scale);
        }

        public void SetScale(Vector3 scale)
        {
            _coordinateSystem.SetScale(scale);
        }

        public void FaceInOppositeDirection()
        {
            _coordinateSystem.InvertAxisScale(CoordinateSystemAxis.PositiveUp);
        }

        public void InheritCoordinateSystem(CoordinateSystem coordinateSystem)
        {
            _coordinateSystem.SetTransformMatrix(coordinateSystem.TransformMatrix);
        }

        public Vector3 GetLocalAxis(CoordinateSystemAxis axis)
        {
            return _coordinateSystem.GetAxisVector(axis);
        }

        public Vector3 GetOriginPosition()
        {
            return _coordinateSystem.GetOriginPosition();
        }

        public Quaternion GetRotation()
        {
            return _coordinateSystem.GetRotation();
        }

        public List<Vector3> GetCenterAndCornerPoints()
        {
            Vector3 center = Center;
            Vector2 scaledXZExtents = ScaledXZExtents;

            Vector3 rightAxis = GetLocalAxis(CoordinateSystemAxis.PositiveRight);
            Vector3 lookAxis = GetLocalAxis(CoordinateSystemAxis.PositiveLook);

            var centerAndCornerPoints = new Vector3[XZOrientedQuad3DPoints.Count];
            centerAndCornerPoints[(int)XZOrientedQuad3DPoint.Center] = center;
            centerAndCornerPoints[(int)XZOrientedQuad3DPoint.TopLeft] = center - rightAxis * scaledXZExtents.x + lookAxis * scaledXZExtents.y;
            centerAndCornerPoints[(int)XZOrientedQuad3DPoint.TopRight] = center + rightAxis * scaledXZExtents.x + lookAxis * scaledXZExtents.y;
            centerAndCornerPoints[(int)XZOrientedQuad3DPoint.BottomRight] = center + rightAxis * scaledXZExtents.x - lookAxis * scaledXZExtents.y;
            centerAndCornerPoints[(int)XZOrientedQuad3DPoint.BottomLeft] = center - rightAxis * scaledXZExtents.x - lookAxis * scaledXZExtents.y;

            return new List<Vector3>(centerAndCornerPoints);
        }

        public List<Vector3> GetMidEdgePoints()
        {
            List<Segment3D> edges = GetBoundarySegments();
            var midEdgePoints = new List<Vector3>();

            foreach(var edge in edges)
            {
                midEdgePoints.Add(edge.GetPoint(0.5f));
            }

            return midEdgePoints;
        }

        public List<Vector3> GetCornerPoints()
        {
            Vector3 center = Center;
            Vector2 scaledXZExtents = ScaledXZExtents;

            Vector3 rightAxis = GetLocalAxis(CoordinateSystemAxis.PositiveRight);
            Vector3 lookAxis = GetLocalAxis(CoordinateSystemAxis.PositiveLook);

            var cornerPoints = new Vector3[XZOrientedQuad3DCornerPoints.Count];
            cornerPoints[(int)XZOrientedQuad3DCornerPoint.TopLeft] = center - rightAxis * scaledXZExtents.x + lookAxis * scaledXZExtents.y;
            cornerPoints[(int)XZOrientedQuad3DCornerPoint.TopRight] = center + rightAxis * scaledXZExtents.x + lookAxis * scaledXZExtents.y;
            cornerPoints[(int)XZOrientedQuad3DCornerPoint.BottomRight] = center + rightAxis * scaledXZExtents.x - lookAxis * scaledXZExtents.y;
            cornerPoints[(int)XZOrientedQuad3DCornerPoint.BottomLeft] = center - rightAxis * scaledXZExtents.x - lookAxis * scaledXZExtents.y;

            return new List<Vector3>(cornerPoints);
        }

        public Vector3 GetPointClosestToPoint(Vector3 point, bool includeMidEdgePoints)
        {
            if (includeMidEdgePoints)
            {
                List<Vector3> points = GetCenterAndCornerPoints();
                points.AddRange(GetMidEdgePoints());
                return Vector3Extensions.GetClosestPointToPoint(points, point);
            }
            else return Vector3Extensions.GetClosestPointToPoint(GetCenterAndCornerPoints(), point);
        }

        public List<Segment3D> GetBoundarySegments()
        {
            return Vector3Extensions.GetSegmentsBetweenPoints(GetCornerPoints(), true);
        }

        public List<Plane> GetBoundarySegmentPlanesFacingOutward()
        {
            List<Segment3D> boundarySegments = GetBoundarySegments();
            List<Plane> segmentPlanes = new List<Plane>(boundarySegments.Count);
            Plane quadPlane = Plane;

            for(int segmentIndex = 0; segmentIndex < boundarySegments.Count; ++segmentIndex)
            {
                Segment3D segment = boundarySegments[segmentIndex];
                Vector3 planeNormal = Vector3.Cross(segment.Direction, quadPlane.normal);
                planeNormal.Normalize();

                segmentPlanes.Add(new Plane(planeNormal, segment.StartPoint));
            }

            return segmentPlanes;
        }
        #endregion
    }
}
#endif