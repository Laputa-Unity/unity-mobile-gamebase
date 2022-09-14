#if UNITY_EDITOR
using UnityEngine;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSnappingLookAndFeelSettingsView : SettingsView
    {
        #region Constructors
        public ObjectSnappingLookAndFeelSettingsView()
        {
            SurroundWithBox = true;
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Look and Feel";
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            ObjectSnapping.Get().XZSnapGrid.RenderSettings.View.Render();
            ObjectSnapping.Get().XZSnapGrid.RenderableCoordinateSystem.RenderSettings.View.Render();
            ObjectSnapping.Get().RenderSettingsForColliderSnapSurfaceGrid.View.Render();
        }
        #endregion
    }
}
#endif