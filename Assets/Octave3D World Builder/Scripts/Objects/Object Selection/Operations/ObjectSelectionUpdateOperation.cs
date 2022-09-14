#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

namespace O3DWB
{
    public abstract class ObjectSelectionUpdateOperation : IObjectSelectionUpdateOperation
    {
        #region Public Abstract Methods
        public abstract void Perform();
        #endregion
    }
}
#endif