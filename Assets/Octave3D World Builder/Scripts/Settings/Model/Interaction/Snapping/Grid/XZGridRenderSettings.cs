#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class XZGridRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isVisible = true;
        [SerializeField]
        private Color _cellLineColor = Color.black;
        [SerializeField]
        private float _cellLineThickness = 0.03f;

        [SerializeField]
        private Color _planeColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

        [SerializeField]
        private XZGridRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinCellLineThickness { get { return 0.01f; } }
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public Color CellLineColor { get { return _cellLineColor; } set { _cellLineColor = value; } }
        public float CellLineThickness { get { return _cellLineThickness; } set { _cellLineThickness = Mathf.Max(MinCellLineThickness, value); } }
        public Color PlaneColor { get { return _planeColor; } set { _planeColor = value; } }
        public XZGridRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public XZGridRenderSettings()
        {
            _view = new XZGridRenderSettingsView(this);
        }
        #endregion
    }
}
#endif
