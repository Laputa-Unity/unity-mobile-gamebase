#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectPlacementSettingsWindow : Octave3DEditorWindow
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        #endregion

        #region Public Static Functions
        public static ObjectPlacementSettingsWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.ObjectPlacementSettingsWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Object Placement Settings";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {          
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            RenderContentInScrollView();
            EditorGUILayout.EndScrollView();
        }
        #endregion

        #region Private Methods
        private void RenderContentInScrollView()
        {
            ObjectPlacementSettings objectPlacementSettings = ObjectPlacementSettings.Get();

            EditorGUILabelWidth.Push(EditorGUILayoutEx.PreferedEditorWindowLabelWidth);
            objectPlacementSettings.ObjectPlacementGuideSettings.View.Render();
            EditorGUILabelWidth.Pop();
        }
        #endregion
    }
}
#endif