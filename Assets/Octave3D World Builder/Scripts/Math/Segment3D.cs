#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public struct Segment3D
    {
        #region Private Variables
        private Vector3 _startPoint;
        private Vector3 _normalizedDirection;
        private Vector3 _direction;
        private float _length;
        private float _sqrLength;
        #endregion

        #region Public Properties
        public Vector3 StartPoint { get { return _startPoint; }  }
        public Vector3 EndPoint { get { return _startPoint + _normalizedDirection * _length; } }
        public Vector3 NormalizedDirection { get { return _normalizedDirection; } }
        public Vector3 Direction { get { return _direction; } }
        public float Length { get { return _length; } }
        public float SqrLength { get { return _sqrLength; } }
        public float HalfLength { get { return _length * 0.5f; } }
        #endregion

        #region Constructors
        public Segment3D(Vector3 startPoint, Vector3 endPoint)
        {
            _startPoint = startPoint;
            _direction = endPoint - _startPoint;
            _length = _direction.magnitude;
            _sqrLength = _direction.sqrMagnitude;
            _normalizedDirection = _direction;
            _normalizedDirection.Normalize();
        }

        public Segment3D(Ray3D ray)
        {
            _startPoint = ray.Origin;
            _direction = ray.Direction;
            _length = _direction.magnitude;
            _sqrLength = _direction.sqrMagnitude;
            _normalizedDirection = _direction;
            _normalizedDirection.Normalize();
        }
        #endregion

        #region Public Methods
        public Vector3 GetRandomPoint()
        {
            float randomT = UnityEngine.Random.Range(0.0f, 1.0f);
            return _startPoint + randomT * _direction;
        }

        public Vector3 GetPoint(float t)
        {
            return _startPoint + t * _direction;
        }

        public bool IsPerpendicualrTo(Segment3D segment)
        {
            return Direction.IsPerpendicularTo(segment.Direction);
        }

        public bool IntersectsWith(Segment3D segment)
        {
            float t1, t2;
            return IntersectsWith(segment, out t1, out t2);
        }

        public bool IntersectsWith(Segment3D segment, out float t1, out float t2)
        {
            t1 = t2 = 0.0f;

            if (_length == 0.0f || segment.Length == 0.0f) return false;

            // If the segments are parallel, they don't intersect
            if (segment.NormalizedDirection.IsAlignedWith(_normalizedDirection)) return false;

            // Check if the 2 segments are coplanar
            Vector3 segmentPlaneNormal = Vector3.Cross(segment.NormalizedDirection, _normalizedDirection);
            Vector3 fromThisSegmentStartToOtherSegmentStart = segment.StartPoint - _startPoint;
            if (!fromThisSegmentStartToOtherSegmentStart.IsPerpendicularTo(segmentPlaneNormal)) return false;

            // Build a plane which slices thorough the segment
            Vector3 slicingPlaneNormal = Vector3.Cross(segmentPlaneNormal, _normalizedDirection);
            slicingPlaneNormal.Normalize();
            Plane slicingPlane = new Plane(slicingPlaneNormal, _startPoint);
            
            // Check if the query segment intersects the plane
            float t;
            Ray3D querySegmentRay = new Ray3D(segment.StartPoint, segment.Direction);
            if (querySegmentRay.IntersectsPlane(slicingPlane, out t))
            {
                // The segment intersect the plane, but we have to ensure that the intersection point lies somewehre along 'this' segment.
                Vector3 intersectionPoint = querySegmentRay.Origin + t * querySegmentRay.Direction;

                Vector3 toIntersectionPoint = intersectionPoint - _startPoint;
                if (Vector3.Dot(toIntersectionPoint, _normalizedDirection) < 0.0f) return false;
                if (toIntersectionPoint.sqrMagnitude > _sqrLength) return false;

                t1 = toIntersectionPoint.magnitude / _length;
                t2 = t;

                return true;
            }

            return false;
        }
        #endregion
    }
}
#endif