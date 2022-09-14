#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace O3DWB
{
    [Serializable]
    public class ObjectGrabSettingsWindow : Octave3DEditorWindow
    {
        #region Private Variables
        [SerializeField]
        private Vector2 _scrollViewPosition = Vector2.zero;
        [NonSerialized]
        private ObjectGrabSettings _objectGrabSettings;
        #endregion

        public ObjectGrabSettings ObjectGrabSettings { set { _objectGrabSettings = value; } }

        #region Public Static Functions
        public static ObjectGrabSettingsWindow Get()
        {
            return Octave3DWorldBuilder.ActiveInstance.EditorWindowPool.SelectionGrabSettingsWindow;
        }
        #endregion

        #region Public Methods
        public override string GetTitle()
        {
            return "Object Grab Settings";
        }

        public override void ShowOctave3DWindow()
        {
            ShowDockable(true);
        }
        #endregion

        #region Protected Methods
        protected override void RenderContent()
        {
            if (_objectGrabSettings == null) return;

            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            _objectGrabSettings.RenderView();
            EditorGUILayout.EndScrollView();
        }
        #endregion
    }
}
#endif