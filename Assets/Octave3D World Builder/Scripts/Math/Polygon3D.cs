#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Polygon3D
    {
        #region Private Variables
        private Vector3 _normal;
        private List<Vector3> _pointsOnPolygonPlane = new List<Vector3>();
        private List<Segment3D> _edges = new List<Segment3D>();
        #endregion

        #region Public Properties
        public Vector3 Normal { get { return _normal; } }
        public Plane Plane { get { return NumberOfPoints != 0 ? new Plane(Normal, _pointsOnPolygonPlane[0]) : new Plane(); } }
        public List<Vector3> PointsOnPolygonPlane { get { return new List<Vector3>(_pointsOnPolygonPlane); } }      // Note: In no particular order!
        public List<Segment3D> Edges { get { return new List<Segment3D>(_edges); } }                                // Note: In no particular order!
        public int NumberOfPoints { get { return _pointsOnPolygonPlane.Count; } }
        #endregion

        #region Public Static Functions
        public static Plane GetPolygonEdgePlane(Segment3D polyEdge, Vector3 polyNormal)
        {
            Vector3 planeNormal = Vector3.Cross(polyEdge.Direction, polyNormal);
            planeNormal.Normalize();

            return new Plane(planeNormal, polyEdge.StartPoint);
        }
        #endregion

        #region Public Methods
        public List<Ray3D> GetEdgeRays()
        {
            if (NumberOfPoints < 3) return new List<Ray3D>();

            var rays = new List<Ray3D>(NumberOfPoints);
            foreach(var segment in _edges)
            {
                rays.Add(new Ray3D(segment.StartPoint, segment.Direction));
            }
          
            return rays;
        }

        public void SetPointsOnPolygonPlaneAndNormal(List<Vector3> pointsOnPolygonPlane, Vector3 polygonNormal)
        {
            _normal = polygonNormal;
            _normal.Normalize();
           
            var quickHull = new Polygon3DQuickHull(pointsOnPolygonPlane, polygonNormal);
            _edges = quickHull.Calculate();

            _pointsOnPolygonPlane.Clear();
            foreach(var edge in _edges)
            {
                _pointsOnPolygonPlane.Add(edge.StartPoint);
                _pointsOnPolygonPlane.Add(edge.EndPoint);
            }
            Vector3Extensions.EliminateDuplicatePoints(_pointsOnPolygonPlane);
        }

        public bool ContainsAnyPoint(List<Vector3> points)
        {
            foreach(Vector3 point in points)
            {
                if (ContainsPoint(point)) return true;
            }

            return false;
        }

        public bool ContainsPoint(Vector3 point)
        {
            if(NumberOfPoints < 3) return false;
            if (!Plane.IsPointOnPlane(point)) return false;
          
            foreach(var edge in _edges)
            {
                Plane edgePlane = GetPolygonEdgePlane(edge, _normal);
                if (edgePlane.IsPointInFront(point)) return false;
            }
        
            return true;
        }
        #endregion
    }
}
#endif