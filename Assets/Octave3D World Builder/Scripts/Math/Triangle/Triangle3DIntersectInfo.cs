#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public class Triangle3DIntersectInfo
    {
        #region Private Variables
        private Triangle3D _firstTriangle;
        private Triangle3D _secondTriangle;

        private List<Vector3> _firstTriangleIntersectionPoints = new List<Vector3>();
        private List<Vector3> _secondTriangleIntersectionPoints = new List<Vector3>();

        private List<Vector3> _allIntersectionPoints = new List<Vector3>();
        #endregion

        #region Public Properties
        public Triangle3D FirstTriangle { get { return _firstTriangle; } }
        public Triangle3D SecondTriangle { get { return _secondTriangle; } }
        public List<Vector3> FirstTriangleIntersectionPoints { get { return new List<Vector3>(_firstTriangleIntersectionPoints); } }
        public List<Vector3> SecondTriangleIntersectionPoints { get { return new List<Vector3>(_secondTriangleIntersectionPoints); } }
        public List<Vector3> AllIntersectionPoints { get { return _allIntersectionPoints; } }
        #endregion

        #region Constructors
        public Triangle3DIntersectInfo()
        {
        }

        public Triangle3DIntersectInfo(Triangle3D firstTriangle, Triangle3D secondTriangle, 
                                       List<Vector3> firstTraingleIntersectionPoints, List<Vector3> secondTraingleIntersectionPoints)
        {
            _firstTriangle = firstTriangle;
            _secondTriangle = secondTriangle;

            _firstTriangleIntersectionPoints = new List<Vector3>(firstTraingleIntersectionPoints);
            _secondTriangleIntersectionPoints = new List<Vector3>(secondTraingleIntersectionPoints);

            _allIntersectionPoints = new List<Vector3>(_firstTriangleIntersectionPoints);
            _allIntersectionPoints.AddRange(_secondTriangleIntersectionPoints);
        }
        #endregion
    }
}
#endif