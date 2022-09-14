#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class Octave3DConfigLoadWindow : Octave3DEditorWindow
    {
        #region Public Static Functions
        public static Octave3DConfigLoadWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ConfigLoadWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Octave3D Config Load";
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
            EditorGUILayout.HelpBox("Please choose the settings you wish to load.", UnityEditor.MessageType.None);
            Octave3DWorldBuilder.ActiveInstance.ConfigLoadSettings.View.Render();
            RenderLoadButton();
        }

        private void RenderLoadButton()
        {
            if (GUILayout.Button(GetContentForLoadButton(), GUILayout.Width(100.0f)))
            {
                string fileName = EditorUtility.OpenFilePanel("Load Octave3D Config", Octave3DWorldBuilder.ActiveInstance.ConfigLoadSettings.LastUsedFolder, "o3dcfg");
                if (!string.IsNullOrEmpty(fileName))
                {
                    Octave3DConfigLoad.LoadConfig(fileName, Octave3DWorldBuilder.ActiveInstance.ConfigLoadSettings);
                    Octave3DWorldBuilder.ActiveInstance.ConfigLoadSettings.LastUsedFolder = FileSystem.GetLastFolderNameInPath(fileName);
                    Octave3DWorldBuilder.ActiveInstance.Inspector.Repaint();
                    EditorUtility.DisplayDialog("Octave3D Config Load", "The configuration was loaded successfully!", "OK");
                }
            }
        }

        private GUIContent GetContentForLoadButton()
        {
            var content = new GUIContent();
            content.text = "Load settings";
            content.tooltip = "Loads the selected settings to a specified file.";

            return content;
        }
        #endregion
    }
}
#endif