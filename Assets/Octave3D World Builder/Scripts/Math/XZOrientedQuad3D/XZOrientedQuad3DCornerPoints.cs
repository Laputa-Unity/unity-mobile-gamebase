#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class XZOrientedQuad3DCornerPoints
    {
        #region Private Static Variables
        private static readonly XZOrientedQuad3DCornerPoint[] _quadPoints;
        private static readonly int _count;
        #endregion

        #region Constructors
        static XZOrientedQuad3DCornerPoints()
        {
            _count = Enum.GetValues(typeof(XZOrientedQuad3DCornerPoint)).Length;

            _quadPoints = new XZOrientedQuad3DCornerPoint[_count];
            _quadPoints[(int)XZOrientedQuad3DCornerPoint.TopLeft] = XZOrientedQuad3DCornerPoint.TopLeft;
            _quadPoints[(int)XZOrientedQuad3DCornerPoint.TopRight] = XZOrientedQuad3DCornerPoint.TopRight;
            _quadPoints[(int)XZOrientedQuad3DCornerPoint.BottomRight] = XZOrientedQuad3DCornerPoint.BottomRight;
            _quadPoints[(int)XZOrientedQuad3DCornerPoint.BottomLeft] = XZOrientedQuad3DCornerPoint.BottomLeft;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<XZOrientedQuad3DCornerPoint> GetAll()
        {
            return new List<XZOrientedQuad3DCornerPoint>(_quadPoints);
        }
        #endregion
    }
}
#endif