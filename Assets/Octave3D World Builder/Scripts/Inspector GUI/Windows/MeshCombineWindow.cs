#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class MeshCombineWindow : Octave3DEditorWindow
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        [NonSerialized]
        private MeshCombineSettings _meshCombineSettings;
        #endregion

        public MeshCombineSettings MeshCombineSettings { set { _meshCombineSettings = value; } }

        #region Public Methods
        public override string GetTitle()
        {
            return "Mesh Combine";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_meshCombineSettings == null) return;

            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            _meshCombineSettings.RenderView();
            EditorGUILayout.EndScrollView();
        }
        #endregion
    }
}
#endif