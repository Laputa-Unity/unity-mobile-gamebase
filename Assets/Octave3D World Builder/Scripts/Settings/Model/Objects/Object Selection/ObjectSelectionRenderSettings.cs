#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRenderSettings : ScriptableObject
    {
        #region Private Variables
        [SerializeField]
        private ObjectSelectionBoxRenderModeSettings _boxRenderModeSettings;

        [SerializeField]
        private ObjectSelectionRenderSettingsView _view;
        #endregion

        #region Public Properties
        public ObjectSelectionBoxRenderModeSettings BoxRenderModeSettings
        {
            get
            {
                if (_boxRenderModeSettings == null) _boxRenderModeSettings = Octave3DWorldBuilder.ActiveInstance.CreateScriptableObject<ObjectSelectionBoxRenderModeSettings>();
                return _boxRenderModeSettings;
            }
        }
        public ObjectSelectionRenderSettingsView View { get { return _view; } }
        #endregion

        #region Constructors
        public ObjectSelectionRenderSettings()
        {
            _view = new ObjectSelectionRenderSettingsView(this);
        }
        #endregion

        #region Public Static Functions
        public static ObjectSelectionRenderSettings Get()
        {
            return ObjectSelection.Get().RenderSettings;
        }
        #endregion
    }
}
#endif