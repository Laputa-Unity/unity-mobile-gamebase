#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class LabelRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _textColor = Color.black;
        [SerializeField]
        private bool _bold = false;
        [SerializeField]
        private int _fontSize = 20;

        [SerializeField]
        private LabelRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static int MinFontSize { get { return 10; } }
        #endregion

        #region Public Properties
        public Color TextColor { get { return _textColor; } set { _textColor = value; } }
        public bool Bold { get { return _bold; } set { _bold = value; } }
        public int FontSize { get { return _fontSize; } set { _fontSize = Mathf.Max(value, MinFontSize); } }
        public LabelRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        private LabelRenderSettings()
        {
            _view = new LabelRenderSettingsView(this);
        }
        #endregion
    }
}
#endif