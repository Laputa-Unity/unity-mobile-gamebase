#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class CoordinateSystemRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _isVisible = true;
        [SerializeField]
        private CoordinateSystemAxisRenderSettings[] _axesRenderSettings;

        [SerializeField]
        private CoordinateSystemRenderSettingsView _view;

        [SerializeField]
        private bool _wasInitialized;
        #endregion

        #region Public Properties
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        public CoordinateSystemRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public CoordinateSystemRenderSettings()
        {
            _view = new CoordinateSystemRenderSettingsView(this);
        }
        #endregion

        #region Public Methods
        public CoordinateSystemAxisRenderSettings GetAxisRenderSettings(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis];
        }

        public bool IsAxisVisible(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis].IsVisible;
        }

        public void SetAxisVisible(CoordinateSystemAxis axis, bool visible)
        {
            _axesRenderSettings[(int)axis].IsVisible = visible;
        }

        public void SetAxisColor(CoordinateSystemAxis axis, Color color)
        {
            _axesRenderSettings[(int)axis].Color = color;
        }

        public Color GetAxisColor(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis].Color;
        }

        public void SetAxisRenderInfinite(CoordinateSystemAxis axis, bool renderInfinite)
        {
            _axesRenderSettings[(int)axis].IsInfinite = renderInfinite;
        }

        public bool IsAxisRenderedInfinite(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis].IsInfinite;
        }

        public void SetAxisFiniteSize(CoordinateSystemAxis axis, float size)
        {
            _axesRenderSettings[(int)axis].FiniteSize = size;
        }

        public float GetAxisFinitSize(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis].FiniteSize;
        }

        /// <summary>
        /// Returns the size of the specified axis. This is a convenience method which 
        /// returns the correct size of the axis based on whether or not the axis is 
        /// rendered infinite. If the axis is not rendered infinite, this is the same
        /// as calling 'GeteAxisFinitSize'.
        /// </summary>
        public float GetAxisSize(CoordinateSystemAxis axis)
        {
            return _axesRenderSettings[(int)axis].GetSize();
        }
        #endregion

        #region Private Methods
        private void OnEnable()
        {
            if (!_wasInitialized)
            {
                InitializeAxesRenderSettingsArray();
                _wasInitialized = true;
            }
        }

        private void InitializeAxesRenderSettingsArray()
        {
            _axesRenderSettings = new CoordinateSystemAxisRenderSettings[CoordinateSystemAxes.Count];
            Color[] axesColors = new Color[] { Color.red, Color.green, Color.blue };

            for(int axisIndex = 0; axisIndex < _axesRenderSettings.Length; ++axisIndex)
            {
                CoordinateSystemAxisRenderSettings axisRenderSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<CoordinateSystemAxisRenderSettings>();
                axisRenderSettings.Axis = (CoordinateSystemAxis)axisIndex;
                _axesRenderSettings[axisIndex] = axisRenderSettings;

                axisRenderSettings.Color = axesColors[axisIndex / 2];
                axisRenderSettings.FiniteSize = 5.0f;
                axisRenderSettings.IsVisible = true;
                axisRenderSettings.IsInfinite = false;
            }
        }
        #endregion
    }
}
#endif