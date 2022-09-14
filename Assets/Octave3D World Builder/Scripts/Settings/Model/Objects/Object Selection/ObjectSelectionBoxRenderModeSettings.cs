#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionBoxRenderModeSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionBoxEdgeRenderMode _edgeRenderMode = ObjectSelectionBoxEdgeRenderMode.CornerEdges;
        [SerializeField]
        private ObjectSelectionBoxCornerEdgesRenderModeSettings _cornerEdgesRenderModeSettings = new ObjectSelectionBoxCornerEdgesRenderModeSettings();

        [SerializeField]
        private Color _edgeColor = Color.white;
        [SerializeField]
        private Color _boxColor = new Color(0.0f, 1.0f, 0.0f, 0.2f);

        [SerializeField]
        private float _boxScale = MinBoxScale;

        [SerializeField]
        private ObjectSelectionBoxRenderModeSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinBoxScale { get { return 1.01f; } }
        #endregion

        #region Public Properties
        public ObjectSelectionBoxEdgeRenderMode EdgeRenderMode { get { return _edgeRenderMode; } set { _edgeRenderMode = value; } }
        public ObjectSelectionBoxCornerEdgesRenderModeSettings CornerEdgesRenderModeSettings { get { return _cornerEdgesRenderModeSettings; } }
        public Color EdgeColor { get { return _edgeColor; } set { _edgeColor = value; } }
        public Color BoxColor { get { return _boxColor; } set { _boxColor = value; } }
        public float BoxScale { get { return _boxScale; } set { _boxScale = Mathf.Max(MinBoxScale, value); } }
        public ObjectSelectionBoxRenderModeSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionBoxRenderModeSettings()
        {
            _view = new ObjectSelectionBoxRenderModeSettingsView(this);
        }
        #endregion
    }
}
#endif