#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class InteractableMirrorSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private InteractableMirrorSettings _settings;
        #endregion

        #region Constructors
        public InteractableMirrorSettingsView(InteractableMirrorSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.MouseOffsetFromHoverSurfaceSettings.View.Render();
            _settings.KeyboardRotationSettings.View.Render();
            _settings.MouseRotationSettings.View.Render();
        }
        #endregion
    }
}
#endif