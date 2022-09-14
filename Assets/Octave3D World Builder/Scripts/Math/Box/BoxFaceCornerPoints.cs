#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class BoxFaceCornerPoints
    {
        #region Private Static Variables
        private static readonly BoxFaceCornerPoint[] _faceCornerPoints;
        private static readonly int _count;
        #endregion

        #region Constructors
        static BoxFaceCornerPoints()
        {
            _count = Enum.GetValues(typeof(BoxFaceCornerPoint)).Length;

            _faceCornerPoints = new BoxFaceCornerPoint[_count];
            _faceCornerPoints[(int)BoxFaceCornerPoint.TopLeft] = BoxFaceCornerPoint.TopLeft;
            _faceCornerPoints[(int)BoxFaceCornerPoint.TopRight] = BoxFaceCornerPoint.TopRight;
            _faceCornerPoints[(int)BoxFaceCornerPoint.BottomRight] = BoxFaceCornerPoint.BottomRight;
            _faceCornerPoints[(int)BoxFaceCornerPoint.BottomLeft] = BoxFaceCornerPoint.BottomLeft;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<BoxFaceCornerPoint> GetAll()
        {
            return new List<BoxFaceCornerPoint>(_faceCornerPoints);
        }
        #endregion
    }
}
#endif