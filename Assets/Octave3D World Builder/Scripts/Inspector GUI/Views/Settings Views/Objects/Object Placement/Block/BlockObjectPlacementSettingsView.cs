#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class BlockObjectPlacementSettingsView : SettingsView
    {
        #region Private Variables
        [NonSerialized]
        private BlockObjectPlacementSettings _settings;
        #endregion

        #region Constructors
        public BlockObjectPlacementSettingsView(BlockObjectPlacementSettings settings)
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