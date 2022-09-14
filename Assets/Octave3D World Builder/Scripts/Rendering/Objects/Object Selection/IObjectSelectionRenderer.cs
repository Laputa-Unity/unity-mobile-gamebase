#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public interface IObjectSelectionRenderer
    {
        #region Interface Methods
        void Render(List<GameObject> selectedObjects);
        #endregion
    }
}
#endif