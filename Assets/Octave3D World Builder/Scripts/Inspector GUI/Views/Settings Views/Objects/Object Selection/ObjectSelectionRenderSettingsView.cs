#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectSelectionRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectSelectionRenderSettingsView(ObjectSelectionRenderSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Selection Rendering";
            IndentContent = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.BoxRenderModeSettings.View.Render();
        }
        #endregion
    }
}
#endif