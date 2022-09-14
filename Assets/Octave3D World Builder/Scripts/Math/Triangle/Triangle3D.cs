#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Triangle3D
    {
        #region Private Variables
        private Vector3[] _points = new Vector3[3];
        private Plane _plane;
        private float _area;
        #endregion

        #region Public Properties
        public Vector3 Point0 { get { return _points[0]; } }
        public Vector3 Point1 { get { return _points[1]; } }
        public Vector3 Point2 { get { return _points[2]; } }
        public Vector3 Normal { get { return _plane.normal; } }
        public Plane Plane { get { return _plane; } }
        public float Area { get { return _area; } }
        public bool IsDegenerate { get { return _area == 0.0f || float.IsNaN(_area); } }
        #endregion

        #region Constructors
        public Triangle3D(Triangle3D source)
        {
            _points = new Vector3[3];
            _points[0] = source.Point0;
            _points[1] = source.Point1;
            _points[2] = source.Point2;
            _plane = source._plane;
            _area = source._area;
        }

        public Triangle3D(Vector3 point0, Vector3 point1, Vector3 point2)
        {
            _points = new Vector3[3];
            _points[0] = point0;
            _points[1] = point1;
            _points[2] = point2;
            CalculateAreaAndPlane();
        }
        #endregion

        #region Public Methods
        public void TransformPoints(TransformMatrix transformMatrix)
        {
            _points[0] = transformMatrix.MultiplyPoint(_points[0]);
            _points[1] = transformMatrix.MultiplyPoint(_points[1]);
            _points[2] = transformMatrix.MultiplyPoint(_points[2]);
            CalculateAreaAndPlane();
        }

        public void TransformPoints(Matrix4x4 transformMatrix)
        {
            _points[0] = transformMatrix.MultiplyPoint(_points[0]);
            _points[1] = transformMatrix.MultiplyPoint(_points[1]);
            _points[2] = transformMatrix.MultiplyPoint(_points[2]);
            CalculateAreaAndPlane();
        }

        public Box GetEncapsulatingBox()
        {
            List<Vector3> points = GetPoints();

            Vector3 minPoint, maxPoint;
            Vector3Extensions.GetMinMaxPoints(points, out minPoint, out maxPoint);

            return new Box((minPoint + maxPoint) * 0.5f, maxPoint - minPoint);
        }

        public bool IntersectsBox(Box box)
        {
            // Store needed data
            Vector3 boxCenter = box.Center;
            float eX = box.Extents.x;
            float eY = box.Extents.y;
            float eZ = box.Extents.z;

            // The triangle points expressed relative to the center of the box
            Vector3 v0 = _points[0] - boxCenter;
            Vector3 v1 = _points[1] - boxCenter;
            Vector3 v2 = _points[2] - boxCenter;

            // Triangle edges
            Vector3 e0 = v1 - v0;
            Vector3 e1 = v2 - v1;
            Vector3 e2 = v0 - v2;

            // Use the separating axis test for all 9 axes which result from crossing the box's local axes
            // with the triangle esges.
            // Test a00 = <1, 0, 0> X e0 = <0, -e0.z, e0.y>
            Vector3 axis = new Vector3(0.0f, -e0.z, e0.y);
            float v0Prj = Vector3.Dot(v0, axis);
            float v1Prj = Vector3.Dot(v1, axis);
            float v2Prj = Vector3.Dot(v2, axis);
            float prjEx = Mathf.Abs(eY * axis.y) + Mathf.Abs(eZ * axis.z);
            float minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            float maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a01 = <1, 0, 0> X e1 = <0, -e1.z, e1.y>
            axis = new Vector3(0.0f, -e1.z, e1.y);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eY * axis.y) + Mathf.Abs(eZ * axis.z);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a02 = <1, 0, 0> X e2 = <0, -e2.z, e2.y>
            axis = new Vector3(0.0f, -e2.z, e2.y);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eY * axis.y) + Mathf.Abs(eZ * axis.z);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a10 = <0, 1, 0> X e0 = <e0.z, 0, -e0.x>
            axis = new Vector3(e0.z, 0.0f, -e0.x);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eZ * axis.z);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a11 = <0, 1, 0> X e1 = <e1.z, 0, -e1.x>
            axis = new Vector3(e1.z, 0.0f, -e1.x);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eZ * axis.z);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a12 = <0, 1, 0> X e2 = <e2.z, 0, -e2.x>
            axis = new Vector3(e2.z, 0.0f, -e2.x);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eZ * axis.z);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a20 = <0, 0, 1> X e0 = <-e0.y, e0.x, 0>
            axis = new Vector3(-e0.y, e0.x, 0.0f);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eY * axis.y);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a21 = <0, 0, 1> X e1 = <-e1.y, e1.x, 0>
            axis = new Vector3(-e1.y, e1.x, 0.0f);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eY * axis.y);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Test a22 = <0, 0, 1> X e2 = <-e2.y, e2.x, 0>
            axis = new Vector3(-e2.y, e2.x, 0.0f);
            v0Prj = Vector3.Dot(v0, axis);
            v1Prj = Vector3.Dot(v1, axis);
            v2Prj = Vector3.Dot(v2, axis);
            prjEx = Mathf.Abs(eX * axis.x) + Mathf.Abs(eY * axis.y);
            minPrj = MathfEx.Min(v0Prj, v1Prj, v2Prj);
            maxPrj = MathfEx.Max(v0Prj, v1Prj, v2Prj);
            if (Mathf.Max(-maxPrj, minPrj) > prjEx) return false;

            // Check if the triangle's box intersects the box
            Box trianlgeBox = GetEncapsulatingBox();
            if (trianlgeBox.IntersectsBox(box, true)) return true;

            // Check if the box spans the triangle plane
            BoxPlaneClassificationResult boxPlaneCalssifyResult = Plane.ClassifyBox(box);
            return (boxPlaneCalssifyResult == BoxPlaneClassificationResult.Spanning || boxPlaneCalssifyResult == BoxPlaneClassificationResult.OnPlane);
        }

        public List<Segment3D> GetSegments()
        {
            var segments = new List<Segment3D>();
            segments.Add(new Segment3D(Point0, Point1));
            segments.Add(new Segment3D(Point1, Point2));
            segments.Add(new Segment3D(Point2, Point0));

            return segments;
        }

        public Plane GetSegmentPlane(int segmentIndex)
        {
            Segment3D segment = GetSegment(segmentIndex);

            Vector3 segmentPlaneNormal = Vector3.Cross(segment.Direction, _plane.normal);
            segmentPlaneNormal.Normalize();
            return new Plane(segmentPlaneNormal, segment.StartPoint);
        }

        public Segment3D GetSegment(int segmentIndex)
        {
            return new Segment3D(_points[segmentIndex], _points[(segmentIndex + 1) % 3]);
        }

        public bool Raycast(Ray ray, out float t)
        {
            if (_plane.Raycast(ray, out t))
            {
                Vector3 intersectionPoint = ray.GetPoint(t);
                return ContainsPoint(intersectionPoint);
            }
            else return false;
        }

        public bool Raycast(Ray3D ray, out float t)
        {
            if(ray.IntersectsPlane(_plane, out t))
            {
                Vector3 intersectionPoint = ray.GetPoint(t);
                return ContainsPoint(intersectionPoint);
            }

            return false;
        }

        public bool ContainsPoint(Vector3 point)
        {
            for(int segmentIndex = 0; segmentIndex < 3; ++segmentIndex)
            {
                Plane segmentPlane = GetSegmentPlane(segmentIndex);
                if (segmentPlane.IsPointInFront(point)) return false;
            }

            return true;
        }

        public Sphere GetEncapsulatingSphere()
        {
            return GetEncapsulatingBox().GetEncpasulatingSphere();
        }

        public Vector3 GetCenter()
        {
            Vector3 pointSum = Point0 + Point1 + Point2;
            return pointSum / 3.0f;
        }

        public List<Vector3> GetPoints()
        {
            return new List<Vector3> { Point0, Point1, Point2 };
        }

        public Vector3 GetPointClosestToPoint(Vector3 point)
        {
            return Vector3Extensions.GetClosestPointToPoint(GetPoints(), point);
        }

        public bool IntersectsTriangle(Triangle3D triangle)
        {
            if (triangle.Normal.IsAlignedWith(Normal)) return false;

            float t;
            List<Segment3D> otherTriSegments = triangle.GetSegments();
            var firstTriIntersectionPoints = new List<Vector3>();
            foreach (var segment in otherTriSegments)
            {
                Ray3D ray = new Ray3D(segment.StartPoint, segment.EndPoint);
                if (Raycast(ray, out t)) firstTriIntersectionPoints.Add(ray.GetPoint(t));
            }

            List<Segment3D> thisTriSegments = GetSegments();
            var secondTriIntersectionPoints = new List<Vector3>();
            foreach (var segment in thisTriSegments)
            {
                Ray3D ray = new Ray3D(segment.StartPoint, segment.EndPoint);
                if (triangle.Raycast(ray, out t)) secondTriIntersectionPoints.Add(ray.GetPoint(t));
            }

            if (firstTriIntersectionPoints.Count != 0 || secondTriIntersectionPoints.Count != 0) return true;
            return false;
        }

        public bool IntersectsTriangle(Triangle3D triangle, out Triangle3DIntersectInfo intersectInfo)
        {
            intersectInfo = new Triangle3DIntersectInfo();
            if (triangle.Normal.IsAlignedWith(Normal)) return false;

            float t;
            List<Segment3D> otherTriSegments = triangle.GetSegments();
            var firstTriIntersectionPoints = new List<Vector3>();
            foreach (var segment in otherTriSegments)
            {
                Ray3D ray = new Ray3D(segment.StartPoint, segment.EndPoint);
                if (Raycast(ray, out t)) firstTriIntersectionPoints.Add(ray.GetPoint(t));
            }

            List<Segment3D> thisTriSegments = GetSegments();
            var secondTriIntersectionPoints = new List<Vector3>();
            foreach(var segment in thisTriSegments)
            {
                Ray3D ray = new Ray3D(segment.StartPoint, segment.EndPoint);
                if (triangle.Raycast(ray, out t)) secondTriIntersectionPoints.Add(ray.GetPoint(t));
            }

            if (firstTriIntersectionPoints.Count != 0 || secondTriIntersectionPoints.Count != 0)
            {
                intersectInfo = new Triangle3DIntersectInfo(this, triangle, firstTriIntersectionPoints, secondTriIntersectionPoints);
                return true;
            }

            return false;
        }
        #endregion

        #region Private Methods
        private void CalculateAreaAndPlane()
        {
            Vector3 edge0 = Point1 - Point0;
            Vector3 edge1 = Point2 - Point0;
            Vector3 normal = Vector3.Cross(edge0, edge1);

            if(normal.magnitude < 1e-5f)
            {
                _area = 0.0f;
                _plane = new Plane(Vector3.zero, Vector3.zero);
            }
            else
            {
                _area = normal.magnitude * 0.5f;

                normal.Normalize();
                _plane = new Plane(normal, Point0);
            }
        }
        #endregion
    }
}
#endif