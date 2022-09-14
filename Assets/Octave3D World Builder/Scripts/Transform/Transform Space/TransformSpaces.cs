#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class TransformSpaces
    {
        #region Private Static Variables
        private static readonly TransformSpace[] _transformSpaces;
        private static readonly int _count;
        #endregion

        #region Constructors
        static TransformSpaces()
        {
            _count = Enum.GetValues(typeof(TransformSpace)).Length;
            _transformSpaces = new TransformSpace[_count];

            InitializeTransformSpacesArray();
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<TransformSpace> GetAll()
        {
            return new List<TransformSpace>(_transformSpaces);
        }
        #endregion

        #region Private Static Functions
        private static void InitializeTransformSpacesArray()
        {
            _transformSpaces[(int)TransformSpace.Global] = TransformSpace.Global;
            _transformSpaces[(int)TransformSpace.Local] = TransformSpace.Local;
        }
        #endregion
    }
}
#endif