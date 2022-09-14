#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class TransformAxes
    {
        #region Private Static Variables
        private static readonly TransformAxis[] _axes;
        private static readonly Vector3[] _globalVectors;
        private static readonly int _count;
        #endregion

        #region Constructors
        static TransformAxes()
        {
            _count = Enum.GetValues(typeof(TransformAxis)).Length;

            _axes = new TransformAxis[_count];
            InitializeTransformAxesArray();

            _globalVectors = new Vector3[_count];
            InitializeGlobalVectorsArray();
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<TransformAxis> GetAll()
        {
            return new List<TransformAxis>(_axes);
        }

        public static Vector3 GetGlobalVector(TransformAxis axis)
        {
            return _globalVectors[(int)axis];
        }

        public static Vector3 GetTransformLocalVector(TransformAxis axis, Transform transform)
        {
            if (axis == TransformAxis.X) return transform.right;
            if (axis == TransformAxis.Y) return transform.up;
            return transform.forward;
        }

        public static Vector3 GetVector(TransformAxis axis, TransformSpace transformSpace, Transform transform)
        {
            if (transformSpace == TransformSpace.Local) return GetTransformLocalVector(axis, transform);
            else return GetGlobalVector(axis);
        }
        #endregion

        #region Private Static Functions
        private static void InitializeTransformAxesArray()
        {
            _axes[(int)TransformAxis.X] = TransformAxis.X;
            _axes[(int)TransformAxis.Y] = TransformAxis.Y;
            _axes[(int)TransformAxis.Z] = TransformAxis.Z;
        }

        private static void InitializeGlobalVectorsArray()
        {
            _globalVectors[(int)TransformAxis.X] = Vector3.right;
            _globalVectors[(int)TransformAxis.Y] = Vector3.up;
            _globalVectors[(int)TransformAxis.Z] = Vector3.forward;
        }
        #endregion
    }
}
#endif