#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementBlockRenderSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private ObjectPlacementBlockRenderSettings _settings;
        #endregion

        #region Constructors
        public ObjectPlacementBlockRenderSettingsView(ObjectPlacementBlockRenderSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.ManualConstructionRenderSettings.View.Render();
        }
        #endregion
    }
}
#endif