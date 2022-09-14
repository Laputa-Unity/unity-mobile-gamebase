#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ProjectedBoxFacePivotPointsRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private SingleObjectPivotPointRenderSettings _activePivotPointRenderSettings;
        [SerializeField]
        private SingleObjectPivotPointRenderSettings _inactivePivotPointRenderSettings;

        [SerializeField]
        private bool _renderProjectionLines = true;
        [SerializeField]
        private Color _projectionLineColor = Color.white;

        [SerializeField]
        private bool _renderPivotPointConnectionLines = true;
        [SerializeField]
        private Color _pivotPointConnectionLineColor = Color.white;

        [SerializeField]
        private bool _wasInitialized = false;

        [SerializeField]
        private ProjectedBoxFacePivotPointsRenderSettingsView _view;
        #endregion

        #region Public Properties
        public SingleObjectPivotPointRenderSettings ActivePivotPointRenderSettings
        { 
            get 
            {
                if (_activePivotPointRenderSettings == null) _activePivotPointRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<SingleObjectPivotPointRenderSettings>();
                return _activePivotPointRenderSettings; 
            } 
        }
        public SingleObjectPivotPointRenderSettings InactivePivotPointRenderSettings 
        { 
            get 
            {
                if (_inactivePivotPointRenderSettings == null) _inactivePivotPointRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<SingleObjectPivotPointRenderSettings>();
                return _inactivePivotPointRenderSettings; 
            } 
        }
        public bool RenderProjectionLines { get { return _renderProjectionLines; } set { _renderProjectionLines = value; } }
        public Color ProjectionLineColor { get { return _projectionLineColor; } set { _projectionLineColor = value; } }
        public bool RenderPivotPointConnectionLines { get { return _renderPivotPointConnectionLines; } set { _renderPivotPointConnectionLines = value; } }
        public Color PivotPointConnectionLineColor { get { return _pivotPointConnectionLineColor; } set { _pivotPointConnectionLineColor = value; } }
        public ProjectedBoxFacePivotPointsRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ProjectedBoxFacePivotPointsRenderSettings()
        {
            _view = new ProjectedBoxFacePivotPointsRenderSettingsView(this);
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            if (!_wasInitialized)
            {
                ActivePivotPointRenderSettings.FillColor = Color.yellow;
                InactivePivotPointRenderSettings.FillColor = Color.white;
                _wasInitialized = true;
            }
        }
        #endregion
    }
}
#endif