#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class SingleObjectPivotPointRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isVisible = true;

        [SerializeField]
        private Color _fillColor = Color.blue;
        [SerializeField]
        private Color _borderLineColor = Color.black;

        [SerializeField]
        private float _scale = 1.0f;

        [SerializeField]
        private SingleObjectPivotPointRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinScale { get { return 0.0f; } }
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public Color FillColor { get { return _fillColor; } set { _fillColor = value; } }
        public Color BorderLineColor { get { return _borderLineColor; } set { _borderLineColor = value; } }
        public float Scale { get { return _scale; } set { _scale = Mathf.Max(MinScale, value); } }
        public SingleObjectPivotPointRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public SingleObjectPivotPointRenderSettings()
        {
            _view = new SingleObjectPivotPointRenderSettingsView(this);
        }
        #endregion
    }
}
#endif