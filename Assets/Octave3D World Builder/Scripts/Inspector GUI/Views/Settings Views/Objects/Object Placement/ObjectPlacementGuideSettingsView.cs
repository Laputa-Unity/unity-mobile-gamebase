#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementGuideSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementGuideSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementGuideSettingsView(ObjectPlacementGuideSettings settings)
        {
            _settings = settings;

            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Object Placement Guide Settings";
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.MouseOffsetFromPlacementSurfaceSettings.View.Render();
            _settings.KeyboardRotationSettings.View.Render();
            _settings.MouseRotationSettings.View.Render();
            _settings.MouseUniformScaleSettings.View.Render();
        }
        #endregion
    }
}
#endif