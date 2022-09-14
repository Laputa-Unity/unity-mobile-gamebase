#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectKeyboardRotationSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectKeyboardRotationSettings _settings;
        #endregion

        #region Constructors
        public ObjectKeyboardRotationSettingsView(ObjectKeyboardRotationSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Keyboard Rotation Settings";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.XAxisRotationSettings.View.Render();
            _settings.YAxisRotationSettings.View.Render();
            _settings.ZAxisRotationSettings.View.Render();
            _settings.CustomAxisRotationSettings.View.Render();
        }
        #endregion
    }
}
#endif