#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Octave3DConfigSaveWindow : Octave3DEditorWindow
    {
        #region Public Static Functions
        public static Octave3DConfigSaveWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ConfigSaveWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Octave3D Config Save";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            RenderContentInScrollView();
        }
        #endregion

        #region Private Methods
        private void RenderContentInScrollView()
        {
            EditorGUILayout.HelpBox("Please choose the settings you wish to save.", UnityEditor.MessageType.None);
            Octave3DWorldBuilder.ActiveInstance.ConfigSaveSettings.View.Render();
            RenderSaveButton();
        }

        private void RenderSaveButton()
        {
            if(GUILayout.Button(GetContentForSaveButton(), GUILayout.Width(100.0f)))
            {
                string fileName = EditorUtility.SaveFilePanel("Save Octave3D Config", Octave3DWorldBuilder.ActiveInstance.ConfigSaveSettings.LastUsedFolder, "", "o3dcfg");
                if (!string.IsNullOrEmpty(fileName))
                {
                    Octave3DConfigSave.SaveConfig(fileName, Octave3DWorldBuilder.ActiveInstance.ConfigSaveSettings);
                    Octave3DWorldBuilder.ActiveInstance.ConfigSaveSettings.LastUsedFolder = FileSystem.GetLastFolderNameInPath(fileName);
                    EditorUtility.DisplayDialog("Octave3D Config Save", "The configuration was saved successfully!", "OK");
                }
            }
        }

        private GUIContent GetContentForSaveButton()
        {
            var content = new GUIContent();
            content.text = "Save settings";
            content.tooltip = "Saves the selected settings to a specified file.";

            return content;
        }
        #endregion
    }
}
#endif