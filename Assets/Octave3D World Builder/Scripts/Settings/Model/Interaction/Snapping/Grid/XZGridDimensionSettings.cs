#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridDimensionSettings
    {
        #region Private Variables
        [SerializeField]
        private XZGridDimensionType _dimensionType = XZGridDimensionType.Infinite;
        [SerializeField]
        private XZGridFiniteDimensionSettings _finiteDimensionSettings = new XZGridFiniteDimensionSettings();
        #endregion

        #region Public Properties
        public XZGridDimensionType DimensionType { get { return _dimensionType; } set { _dimensionType = value; } }
        public XZGridFiniteDimensionSettings FiniteDimensionSettings { get { return _finiteDimensionSettings; } }
        #endregion
    }
}
#endif