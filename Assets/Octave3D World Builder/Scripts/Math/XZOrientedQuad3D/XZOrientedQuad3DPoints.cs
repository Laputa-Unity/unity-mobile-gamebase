#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class XZOrientedQuad3DPoints 
    {
	    #region Private Static Variables
        private static readonly XZOrientedQuad3DPoint[] _quadPoints;
        private static readonly int _count;
        #endregion

        #region Constructors
        static XZOrientedQuad3DPoints()
        {
            _count = Enum.GetValues(typeof(XZOrientedQuad3DPoint)).Length;

            _quadPoints = new XZOrientedQuad3DPoint[_count];
            _quadPoints[(int)XZOrientedQuad3DPoint.Center] = XZOrientedQuad3DPoint.Center;
            _quadPoints[(int)XZOrientedQuad3DPoint.TopLeft] = XZOrientedQuad3DPoint.TopLeft;
            _quadPoints[(int)XZOrientedQuad3DPoint.TopRight] = XZOrientedQuad3DPoint.TopRight;
            _quadPoints[(int)XZOrientedQuad3DPoint.BottomRight] = XZOrientedQuad3DPoint.BottomRight;
            _quadPoints[(int)XZOrientedQuad3DPoint.BottomLeft] = XZOrientedQuad3DPoint.BottomLeft;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<XZOrientedQuad3DPoint> GetAll()
        {
            return new List<XZOrientedQuad3DPoint>(_quadPoints);
        }
        #endregion
    }
}
#endif