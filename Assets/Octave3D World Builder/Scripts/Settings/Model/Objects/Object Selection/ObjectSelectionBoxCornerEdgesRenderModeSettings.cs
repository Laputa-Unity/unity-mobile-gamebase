#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionBoxCornerEdgesRenderModeSettings
    {
        #region Private Variables
        [SerializeField]
        private float _cornerEdgeLengthPercentage = 0.3f;

        [SerializeField]
        private ObjectSelectionBoxCornerEdgesRenderModeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinCornerEdgeLengthPercentage { get { return 0.01f; } }
        public static float MaxCornerEdgeLengthPercentage { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public float CornerEdgeLengthPercentage { get { return _cornerEdgeLengthPercentage; } set { _cornerEdgeLengthPercentage = Mathf.Clamp(value, MinCornerEdgeLengthPercentage, MaxCornerEdgeLengthPercentage); } }
        public ObjectSelectionBoxCornerEdgesRenderModeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionBoxCornerEdgesRenderModeSettings()
        {
            _view = new ObjectSelectionBoxCornerEdgesRenderModeSettingsView(this);
        }
        #endregion
    }
}
#endif