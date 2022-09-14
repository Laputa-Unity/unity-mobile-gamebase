#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class BoxCornerPoints
    {
        #region Private Static Variables
        private static readonly BoxCornerPoint[] _cornerPoints;
        private static readonly int _count;
        #endregion

        #region Constructors
        static BoxCornerPoints()
        {
            _count = Enum.GetValues(typeof(BoxCornerPoint)).Length;

            _cornerPoints = new BoxCornerPoint[_count];
            _cornerPoints[(int)BoxCornerPoint.FrontBottomLeft] = BoxCornerPoint.FrontBottomLeft;
            _cornerPoints[(int)BoxCornerPoint.FrontBottomRight] = BoxCornerPoint.FrontBottomRight;
            _cornerPoints[(int)BoxCornerPoint.FrontTopLeft] = BoxCornerPoint.FrontTopLeft;
            _cornerPoints[(int)BoxCornerPoint.FrontTopRight] = BoxCornerPoint.FrontTopRight;
            _cornerPoints[(int)BoxCornerPoint.BackBottomLeft] = BoxCornerPoint.BackBottomLeft;
            _cornerPoints[(int)BoxCornerPoint.BackBottomRight] = BoxCornerPoint.BackBottomRight;
            _cornerPoints[(int)BoxCornerPoint.BackTopLeft] = BoxCornerPoint.BackTopLeft;
            _cornerPoints[(int)BoxCornerPoint.BackTopRight] = BoxCornerPoint.BackTopRight;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<BoxCornerPoint> GetAll()
        {
            return new List<BoxCornerPoint>(_cornerPoints);
        }
        #endregion
    }
}
#endif