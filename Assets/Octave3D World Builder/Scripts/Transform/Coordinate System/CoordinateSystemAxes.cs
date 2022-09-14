#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class CoordinateSystemAxes
    {
        #region Private Static Variables
        private static readonly CoordinateSystemAxis[] _axes;
        private static readonly Vector3[] _globalVectors;
        private static readonly int _count;
        #endregion

        #region Constructors
        static CoordinateSystemAxes()
        {
            _count = Enum.GetValues(typeof(CoordinateSystemAxis)).Length;
           
            _axes = new CoordinateSystemAxis[_count];
            InitializeAxesArray();

            _globalVectors = new Vector3[_count];
            InitializeGlobalVectorsArray();
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<CoordinateSystemAxis> GetAll()
        {
            return new List<CoordinateSystemAxis>(_axes);
        }

        public static Vector3 GetGlobalVector(CoordinateSystemAxis axis)
        {
            return _globalVectors[(int)axis];
        }

        public static Vector3 GetLocalVector(CoordinateSystemAxis axis, Transform transform)
        {
            if (axis == CoordinateSystemAxis.PositiveRight) return transform.right;
            if (axis == CoordinateSystemAxis.NegativeRight) return -transform.right;
            if (axis == CoordinateSystemAxis.PositiveUp) return transform.up;
            if (axis == CoordinateSystemAxis.NegativeUp) return -transform.up;
            if (axis == CoordinateSystemAxis.PositiveLook) return transform.forward;
            return -transform.forward;
        }

        public static Vector3 GetLocalVector(CoordinateSystemAxis axis, TransformMatrix transformMatrix)
        {
            if (axis == CoordinateSystemAxis.PositiveRight) return transformMatrix.GetNormalizedRightAxis();
            if (axis == CoordinateSystemAxis.NegativeRight) return -transformMatrix.GetNormalizedRightAxis();
            if (axis == CoordinateSystemAxis.PositiveUp) return transformMatrix.GetNormalizedUpAxis();
            if (axis == CoordinateSystemAxis.NegativeUp) return -transformMatrix.GetNormalizedUpAxis();
            if (axis == CoordinateSystemAxis.PositiveLook) return transformMatrix.GetNormalizedLookAxis();
            return -transformMatrix.GetNormalizedLookAxis();
        }

        public static Vector3 GetVector(CoordinateSystemAxis axis, TransformSpace transformSpace, Transform transform)
        {
            if (transformSpace == TransformSpace.Local) return GetLocalVector(axis, transform);
            else return GetGlobalVector(axis);
        }

        public static bool IsNegativeAxis(CoordinateSystemAxis axis)
        {
            return axis == CoordinateSystemAxis.NegativeRight || axis == CoordinateSystemAxis.NegativeUp || axis == CoordinateSystemAxis.NegativeLook;
        }

        public static CoordinateSystemAxis GetNext(CoordinateSystemAxis axis)
        {
            return (CoordinateSystemAxis)(((int)axis + 1) % Count);
        }
        #endregion

        #region Private Static Functions
        private static void InitializeAxesArray()
        {
            _axes[(int)CoordinateSystemAxis.PositiveRight] = CoordinateSystemAxis.PositiveRight;
            _axes[(int)CoordinateSystemAxis.NegativeRight] = CoordinateSystemAxis.NegativeRight;
            _axes[(int)CoordinateSystemAxis.PositiveUp] = CoordinateSystemAxis.PositiveUp;
            _axes[(int)CoordinateSystemAxis.NegativeUp] = CoordinateSystemAxis.NegativeUp;
            _axes[(int)CoordinateSystemAxis.PositiveLook] = CoordinateSystemAxis.PositiveLook;
            _axes[(int)CoordinateSystemAxis.NegativeLook] = CoordinateSystemAxis.NegativeLook;
        }

        private static void InitializeGlobalVectorsArray()
        {
            _globalVectors[(int)CoordinateSystemAxis.PositiveRight] = Vector3.right;
            _globalVectors[(int)CoordinateSystemAxis.NegativeRight] = Vector3.left;
            _globalVectors[(int)CoordinateSystemAxis.PositiveUp] = Vector3.up;
            _globalVectors[(int)CoordinateSystemAxis.NegativeUp] = Vector3.down;
            _globalVectors[(int)CoordinateSystemAxis.PositiveLook] = Vector3.forward;
            _globalVectors[(int)CoordinateSystemAxis.NegativeLook] = Vector3.back;
        }
        #endregion
    }
}
#endif