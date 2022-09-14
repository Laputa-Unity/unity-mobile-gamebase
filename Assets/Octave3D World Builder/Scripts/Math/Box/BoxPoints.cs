#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class BoxPoints
    {
        #region Private Static Variables
        private static readonly BoxPoint[] _points;
        private static readonly int _count;
        #endregion

        #region Constructors
        static BoxPoints()
        {
            _count = Enum.GetValues(typeof(BoxPoint)).Length;

            _points = new BoxPoint[_count];
            _points[(int)BoxPoint.Center] = BoxPoint.Center;
            _points[(int)BoxPoint.FrontBottomLeft] = BoxPoint.FrontBottomLeft;
            _points[(int)BoxPoint.FrontBottomRight] = BoxPoint.FrontBottomRight;
            _points[(int)BoxPoint.FrontTopLeft] = BoxPoint.FrontTopLeft;
            _points[(int)BoxPoint.FrontTopRight] = BoxPoint.FrontTopRight;
            _points[(int)BoxPoint.BackBottomLeft] = BoxPoint.BackBottomLeft;
            _points[(int)BoxPoint.BackBottomRight] = BoxPoint.BackBottomRight;
            _points[(int)BoxPoint.BackTopLeft] = BoxPoint.BackTopLeft;
            _points[(int)BoxPoint.BackTopRight] = BoxPoint.BackTopRight;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<BoxPoint> GetAll()
        {
            return new List<BoxPoint>(_points);
        }
        #endregion
    }
}
#endif