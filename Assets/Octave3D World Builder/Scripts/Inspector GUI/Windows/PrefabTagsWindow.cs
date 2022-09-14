#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace O3DWB
{
    public class PrefabTagsWindow : Octave3DEditorWindow
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        #endregion

        #region Public Static Functions
        public static PrefabTagsWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.PrefabTagsWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Prefab Tags";
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
            EditorGUILabelWidth.Push(EditorGUILayoutEx.PreferedEditorWindowLabelWidth);
            PrefabTagDatabase.Get().View.Render();
            EditorGUILabelWidth.Pop();
        }
        #endregion
    }
}
#endif