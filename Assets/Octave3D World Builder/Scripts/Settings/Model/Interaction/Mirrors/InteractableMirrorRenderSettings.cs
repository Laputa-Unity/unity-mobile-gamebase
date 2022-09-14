#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirrorRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private Color _mirrorPlaneColor = new Color(1.0f, 1.0f, 0.0f, 0.7f);
        [SerializeField]
        private Color _mirrorPlaneBorderLineColor = Color.black;
        [SerializeField]
        private float _mirrorWidth = 10.0f;
        [SerializeField]
        private float _mirrorHeight = 10.0f;

        [SerializeField]
        private bool _useInfiniteWidth = false;
        [SerializeField]
        private bool _useInfiniteHeight = false;

        [SerializeField]
        private Color _mirroredBoxColor = new Color(1.0f, 1.0f, 0.0f, 0.7f);
        [SerializeField]
        private Color _mirroredBoxBorderLineColor = Color.black;

        [SerializeField]
        private InteractableMirrorRenderSettingsView _view;
        #endregion

        #region Public Static Properties
        public static float MinMirrorSize { get { return 1.0f; } }
        #endregion

        #region Public Properties
        public Color MirrorPlaneColor { get { return _mirrorPlaneColor; } set { _mirrorPlaneColor = value; } }
        public Color MirrorPlaneBorderLineColor { get { return _mirrorPlaneBorderLineColor; } set { _mirrorPlaneBorderLineColor = value; } }
        public float MirrorWidth { get { return _mirrorWidth; } set { _mirrorWidth = Mathf.Max(value, MinMirrorSize); } }
        public float MirrorHeight { get { return _mirrorHeight; } set { _mirrorHeight = Mathf.Max(value, MinMirrorSize); } }
        public bool UseInfiniteWidth { get { return _useInfiniteWidth; } set { _useInfiniteWidth = value; } }
        public bool UseInfiniteHeight { get { return _useInfiniteHeight; } set { _useInfiniteHeight = value; } }
        public Color MirroredBoxColor { get { return _mirroredBoxColor; } set { _mirroredBoxColor = value; } }
        public Color MirroredBoxBorderLineColor { get { return _mirroredBoxBorderLineColor; } set { _mirroredBoxBorderLineColor = value; } }
        public InteractableMirrorRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        private InteractableMirrorRenderSettings()
        {
            _view = new InteractableMirrorRenderSettingsView(this);
        }
        #endregion
    }
}
#endif