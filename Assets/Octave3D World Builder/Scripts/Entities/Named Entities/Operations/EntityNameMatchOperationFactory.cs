#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class EntityNameMatchOperationFactory
    {
        #region Public Static Functions
        public static IEntityNameMatchOperation Create(bool caseSensitiveMatch)
        {
            if (caseSensitiveMatch) return new CaseSensitiveEntityNameMatchOperation();
            return new CaseInsensitiveEntityNameMatchOperation();
        }
        #endregion
    }
}
#endif