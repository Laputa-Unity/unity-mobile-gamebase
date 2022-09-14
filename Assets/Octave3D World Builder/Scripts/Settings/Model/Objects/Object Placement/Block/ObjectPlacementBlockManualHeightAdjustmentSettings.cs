#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockManualHeightAdjustmentSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private int _raiseAmount = 1;
        [SerializeField]
        private int _lowerAmount = 1;

        [SerializeField]
        private ObjectPlacementBlockManualHeightAdjustmentSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinRaiseAmount { get { return 1; } }
        public static int MinLowerAmount { get { return 1; } }
        #endregion

        #region Public Properties
        public int RaiseAmount { get { return _raiseAmount; } set { _raiseAmount = Mathf.Max(value, MinRaiseAmount); } }
        public int LowerAmount { get { return _lowerAmount; } set { _lowerAmount = Mathf.Max(value, MinLowerAmount); } }
        public ObjectPlacementBlockManualHeightAdjustmentSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementBlockManualHeightAdjustmentSettings()
        {
            _view = new ObjectPlacementBlockManualHeightAdjustmentSettingsView(this);
        }
        #endregion
    }
}
#endif