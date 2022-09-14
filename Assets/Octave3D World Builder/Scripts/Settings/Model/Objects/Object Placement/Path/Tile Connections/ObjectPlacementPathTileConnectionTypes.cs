#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectPlacementPathTileConnectionTypes
    {
        #region Private Static Variables
        private static readonly ObjectPlacementPathTileConnectionType[] _types;
        private static readonly int _count;
        #endregion

        #region Constructors
        static ObjectPlacementPathTileConnectionTypes()
        {
            _count = Enum.GetValues(typeof(ObjectPlacementPathTileConnectionType)).Length;

            _types = new ObjectPlacementPathTileConnectionType[_count];
            _types[(int)ObjectPlacementPathTileConnectionType.Begin] = ObjectPlacementPathTileConnectionType.Begin;
            _types[(int)ObjectPlacementPathTileConnectionType.End] = ObjectPlacementPathTileConnectionType.End;
            _types[(int)ObjectPlacementPathTileConnectionType.Forward] = ObjectPlacementPathTileConnectionType.Forward;
            _types[(int)ObjectPlacementPathTileConnectionType.TJunction] = ObjectPlacementPathTileConnectionType.TJunction;
            _types[(int)ObjectPlacementPathTileConnectionType.Cross] = ObjectPlacementPathTileConnectionType.Cross;
            _types[(int)ObjectPlacementPathTileConnectionType.Turn] = ObjectPlacementPathTileConnectionType.Turn;
            _types[(int)ObjectPlacementPathTileConnectionType.Autofill] = ObjectPlacementPathTileConnectionType.Autofill;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<ObjectPlacementPathTileConnectionType> GetAll()
        {
            return new List<ObjectPlacementPathTileConnectionType>(_types);
        }

        public static List<ObjectPlacementPathTileConnectionType> GetAllExceptAutofill()
        {
            List<ObjectPlacementPathTileConnectionType> tileConnectionTypes = GetAll();
            tileConnectionTypes.RemoveAt(Count - 1);

            return tileConnectionTypes;
        }
        #endregion
    }
}
#endif