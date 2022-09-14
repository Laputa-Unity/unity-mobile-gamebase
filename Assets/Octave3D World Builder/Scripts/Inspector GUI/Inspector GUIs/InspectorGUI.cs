#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public abstract class InspectorGUI
    {
        #region Public Methods
        public virtual void Initialize()
        {
        }
        #endregion

        #region Public Abstract Methods
        public abstract void Render();
        #endregion
    }
}
#endif