#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectScaleRandomizationSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private bool _randomizeScale = false;

        [SerializeField]
        private ObjectUniformScaleRandomizationSettings _uniformScaleRandomizationSettings;

        [SerializeField]
        private ObjectScaleRandomizationSettingsView _view;
        #endregion

        #region Public Properties
        public bool RandomizeScale { get { return _randomizeScale; } set { _randomizeScale = value; } }
        public ObjectUniformScaleRandomizationSettings UniformScaleRandomizationSettings
        {
            get
            {
                if (_uniformScaleRandomizationSettings == null) _uniformScaleRandomizationSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectUniformScaleRandomizationSettings>();
                return _uniformScaleRandomizationSettings;
            }
        }
        public ObjectScaleRandomizationSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectScaleRandomizationSettings()
        {
            _view = new ObjectScaleRandomizationSettingsView(this);
        }
        #endregion

        #region Private Methods
        private void OnDestroy()
        {
            if (_uniformScaleRandomizationSettings != null) UndoEx.DestroyObjectImmediate(_uniformScaleRandomizationSettings);
        }
        #endregion
    }
}
#endif