#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public abstract class ObjectSelectionRenderer : IObjectSelectionRenderer
    {
        #region Public Abstract Methods
        public abstract void Render(List<GameObject> selectedObjects);
        #endregion
    }
}
#endif