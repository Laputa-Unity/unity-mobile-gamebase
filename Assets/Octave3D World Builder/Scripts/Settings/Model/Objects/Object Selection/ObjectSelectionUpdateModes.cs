#if UNITY_EDITOR
using UnityEngine;
using System;
using System.Collections.Generic;

namespace O3DWB
{
    public static class ObjectSelectionUpdateModes
    {
        #region Private Static Variables
        private static readonly ObjectSelectionUpdateMode[] _selectionUpdateModes;
        private static readonly int _count;
        #endregion

        #region Constructors
        static ObjectSelectionUpdateModes()
        {
            _count = Enum.GetValues(typeof(ObjectSelectionUpdateMode)).Length;

            _selectionUpdateModes = new ObjectSelectionUpdateMode[_count];
            _selectionUpdateModes[(int)ObjectSelectionUpdateMode.SingleObject] = ObjectSelectionUpdateMode.SingleObject;
            _selectionUpdateModes[(int)ObjectSelectionUpdateMode.EntireHierarchy] = ObjectSelectionUpdateMode.EntireHierarchy;
        }
        #endregion

        #region Public Static Properties
        public static int Count { get { return _count; } }
        #endregion

        #region Public Static Functions
        public static List<ObjectSelectionUpdateMode> GetAll()
        {
            return new List<ObjectSelectionUpdateMode>(_selectionUpdateModes);
        }

        public static ObjectSelectionUpdateMode GetNext(ObjectSelectionUpdateMode selectionUpdateMode)
        {
            return (ObjectSelectionUpdateMode)(((int)selectionUpdateMode + 1) % _count);
        }
        #endregion
    }
}
#endif