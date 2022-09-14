#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementExtensionPlaneRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _planeColor = new Color(0.101f, 1.0f, 0.51f, 0.4f);
        [SerializeField]
        private Color _planeBorderLineColor = Color.black;

        [SerializeField]
        private float _planeNormalLineLength = 1.0f;
        [SerializeField]
        private Color _planeNormalLineColor = Color.white;

        [SerializeField]
        private float _planeScale = 1.8f;

        [SerializeField]
        private ObjectPlacementExtensionPlaneRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinPlaneNormalLineLength { get { return 0.0f; } }
        public static float MinScale { get { return 1.08f; } }
        #endregion

        #region Public Properties
        public Color PlaneColor { get { return _planeColor; } set { _planeColor = value; } }
        public Color PlaneBorderLineColor { get { return _planeBorderLineColor; } set { _planeBorderLineColor = value; } }
        public float PlaneNormalLineLength { get { return _planeNormalLineLength; } set { _planeNormalLineLength = Mathf.Max(value, MinPlaneNormalLineLength); } }
        public Color PlaneNormalLineColor { get { return _planeNormalLineColor; } set { _planeNormalLineColor = value; } }
        public float PlaneScale { get { return _planeScale; } set { _planeScale = Mathf.Max(value, MinScale); } }
        public ObjectPlacementExtensionPlaneRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectPlacementExtensionPlaneRenderSettings()
        {
            _view = new ObjectPlacementExtensionPlaneRenderSettingsView(this);
        }
        #endregion
    }
}
#endif