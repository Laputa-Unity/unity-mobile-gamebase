#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class BoxFacePoints
    {
        #region Private Static Variables
        private static readonly BoxFacePoint[] _facePoints;
        private static readonly int _count;
        #endregion

        #region Constructors
        static BoxFacePoints()
        {
            _count = Enum.GetValues(typeof(BoxFacePoint)).Length;

            _facePoints = new BoxFacePoint[_count];
            _facePoints[(int)BoxFacePoint.Center] = BoxFacePoint.Center;
            _facePoints[(int)BoxFacePoint.TopLeft] = BoxFacePoint.TopLeft;
            _facePoints[(int)BoxFacePoint.TopRight] = BoxFacePoint.TopRight;
            _facePoints[(int)BoxFacePoint.BottomRight] = BoxFacePoint.BottomRight;
            _facePoints[(int)BoxFacePoint.BottomLeft] = BoxFacePoint.BottomLeft;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<BoxFacePoint> GetAll()
        {
            return new List<BoxFacePoint>(_facePoints);
        }
        #endregion
    }
}
#endif