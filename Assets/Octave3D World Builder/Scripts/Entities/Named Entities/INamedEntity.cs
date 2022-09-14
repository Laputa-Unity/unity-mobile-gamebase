#if UNITY_EDITOR
using UnityEngine;

namespace O3DWB
{
    public interface INamedEntity
    {
        #region Interface Properties
        string Name { get; set; }
        #endregion
    }
}
#endif