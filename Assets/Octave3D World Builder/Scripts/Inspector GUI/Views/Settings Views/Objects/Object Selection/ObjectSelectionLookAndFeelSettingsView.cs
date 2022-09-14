#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectSelectionLookAndFeelSettingsView : SettingsView
    {
        #region Constructors
        public ObjectSelectionLookAndFeelSettingsView()
        {
            ToggleVisibilityBeforeRender = true;
            VisibilityToggleLabel = "Look and Feel";
            IndentContent = true;
            SurroundWithBox = true;
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            ObjectSelection objectSelection = ObjectSelection.Get();
            objectSelection.RectangleSelectionShapeRenderSettings.View.Render();
            objectSelection.EllipseSelectionShapeRenderSettings.View.Render();
            ObjectSelectionRenderSettings.Get().View.Render();
        }
        #endregion
    }
}
#endif