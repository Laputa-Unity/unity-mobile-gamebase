#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    public static class TypeExtensions
    {
        #region Public Static Functions
        public static bool IsNumeric(this Type type)
        {
            if (!type.IsPrimitive || type == typeof(bool) || type == typeof(char)) return false;
            return true;
        }
        #endregion
    }
}
#endif