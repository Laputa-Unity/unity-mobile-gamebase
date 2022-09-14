#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public static class ObjectSelectionRendererFactory
    {
        #region Public Static Functions
        public static IObjectSelectionRenderer Create()
        {
            return new BoxObjectSelectionRenderer();
        }
        #endregion
    }
}
#endif