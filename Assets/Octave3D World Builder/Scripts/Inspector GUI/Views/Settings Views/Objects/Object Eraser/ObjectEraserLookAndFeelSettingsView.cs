#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectEraserLookAndFeelSettingsView : SettingsView
    {
        #region Constructors
        public ObjectEraserLookAndFeelSettingsView()
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
            ObjectEraser objectEraser = ObjectEraser.Get();

            objectEraser.Circle2DMassEraseShapeRenderSettings.View.Render();
            objectEraser.Circle3DMassEraseShapeRenderSettings.View.Render();
        }
        #endregion
    }
}
#endif