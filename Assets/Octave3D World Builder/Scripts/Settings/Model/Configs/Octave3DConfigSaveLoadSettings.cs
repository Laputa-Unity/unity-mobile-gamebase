#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class Octave3DConfigSaveLoadSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private string _lastUsedFolder = "";

        [SerializeField]
        private bool _snapSettings = true;
        [SerializeField]
        private bool _objectSelectionSettings = true;
        [SerializeField]
        private bool _objectErasingSettings = true;
        [SerializeField]
        private bool _mirrorLookAndFeel = true;
        [SerializeField]
        private bool _snapLookAndFeel = true;
        [SerializeField]
        private bool _objectPlacementLookAndFeel = true;
        [SerializeField]
        private bool _objectSelectionLookAndFeel = true;
        [SerializeField]
        private bool _objectErasingLookAndFeel = true;

        [SerializeField]
        private Octave3DConfigSaveLoadSettingsView _view;
        #endregion

        #region Public Properties
        public string LastUsedFolder { get { return _lastUsedFolder; } set { if (value != null) _lastUsedFolder = value; } }
        public bool SnapSettings { get { return _snapSettings; } set { _snapSettings = value; } }
        public bool ObjectSelectionSettings { get { return _objectSelectionSettings; } set { _objectSelectionSettings = value; } }
        public bool ObjectErasingSettings { get { return _objectErasingSettings; } set { _objectErasingSettings = value; } }
        public bool MirrorLookAndFeel { get { return _mirrorLookAndFeel; } set { _mirrorLookAndFeel = value; } }
        public bool SnapLookAndFeel { get { return _snapLookAndFeel; } set { _snapLookAndFeel = value; } }
        public bool ObjectPlacementLookAndFeel { get { return _objectPlacementLookAndFeel; } set { _objectPlacementLookAndFeel = value; } }
        public bool ObjectSelectionLookAndFeel { get { return _objectSelectionLookAndFeel; } set { _objectSelectionLookAndFeel = value; } }
        public bool ObjectErasingLookAndFeel { get { return _objectErasingLookAndFeel; } set { _objectErasingLookAndFeel = value; } }
        public Octave3DConfigSaveLoadSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public Octave3DConfigSaveLoadSettings()
        {
            _view = new Octave3DConfigSaveLoadSettingsView(this);
        }
        #endregion

        #region Public Methods
        public void ToggleAll(bool on)
        {
            _snapSettings = on;
            _objectSelectionSettings = on;
            _objectErasingSettings = on;

            _mirrorLookAndFeel = on;
            _snapLookAndFeel = on;
            _objectPlacementLookAndFeel = on;
            _objectSelectionLookAndFeel = on;
            _objectErasingLookAndFeel = on;
        }
        #endregion
    }
}
#endif