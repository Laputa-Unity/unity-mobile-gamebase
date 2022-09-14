#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public class ObjectPlacementPathTileConnectionYAxisRotations 
    {
        #region Private Static Variables
        private static readonly ObjectPlacementPathTileConnectionYAxisRotation[] _yAxisRotations;
        private static readonly float[] _rotationAnglesInDegrees;
        private static readonly int _count;
        #endregion

        #region Constructors
        static ObjectPlacementPathTileConnectionYAxisRotations()
        {
            _count = Enum.GetValues(typeof(ObjectPlacementPathTileConnectionYAxisRotation)).Length;

            _yAxisRotations = new ObjectPlacementPathTileConnectionYAxisRotation[_count];
            _yAxisRotations[(int)ObjectPlacementPathTileConnectionYAxisRotation._0] = ObjectPlacementPathTileConnectionYAxisRotation._0;
            _yAxisRotations[(int)ObjectPlacementPathTileConnectionYAxisRotation._90] = ObjectPlacementPathTileConnectionYAxisRotation._90;
            _yAxisRotations[(int)ObjectPlacementPathTileConnectionYAxisRotation._180] = ObjectPlacementPathTileConnectionYAxisRotation._180;
            _yAxisRotations[(int)ObjectPlacementPathTileConnectionYAxisRotation._270] = ObjectPlacementPathTileConnectionYAxisRotation._270;

            _rotationAnglesInDegrees = new float[_count];
            _rotationAnglesInDegrees[(int)ObjectPlacementPathTileConnectionYAxisRotation._0] = 0.0f;
            _rotationAnglesInDegrees[(int)ObjectPlacementPathTileConnectionYAxisRotation._90] = 90.0f;
            _rotationAnglesInDegrees[(int)ObjectPlacementPathTileConnectionYAxisRotation._180] = 180.0f;
            _rotationAnglesInDegrees[(int)ObjectPlacementPathTileConnectionYAxisRotation._270] = 270.0f;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<ObjectPlacementPathTileConnectionYAxisRotation> GetAll()
        {
            return new List<ObjectPlacementPathTileConnectionYAxisRotation>(_yAxisRotations);
        }

        public static float GetAngleInDegrees(ObjectPlacementPathTileConnectionYAxisRotation yAxisRotation)
        {
            return _rotationAnglesInDegrees[(int)yAxisRotation];
        }
        #endregion
    }
}
#endif