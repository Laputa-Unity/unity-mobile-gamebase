#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PathObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private PathObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public PathObjectPlacementSettingsView(PathObjectPlacementSettings settings)
        {
            _settings = settings;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            _settings.PlacementGuideSurfaceAlignmentSettings.View.Render();
        }
        #endregion
    }
}
#endif