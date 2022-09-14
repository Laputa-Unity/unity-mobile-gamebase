#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class PathObjectPlacementLookAndFeelSettingsView : SettingsView
    {
        #region Constructors
        public PathObjectPlacementLookAndFeelSettingsView()
        {
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            PathObjectPlacement.Get().PathRenderSettings.View.Render();
            PathObjectPlacement.Get().PathExtensionPlaneRenderSettings.View.Render();
        }
        #endregion
    }
}
#endif